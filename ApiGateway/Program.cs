using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
Console.WriteLine($"[INFO] Secret Key: {builder.Configuration["Jwt:Key"]}");
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        };

        // Логируем ошибки
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"-->[ERROR] JWT validation failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("-->[INFO] Токен успешно валиден");
                return Task.CompletedTask;
            }
        };
    });


var app = builder.Build();
// app.UseHttpsRedirection();
Console.WriteLine("HELLO");
// Пропускаем /auth/sign-in без авторизации
app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/api/auth/sign-in"),
    middlewareBuilder => middlewareBuilder.Use((context, next) => next())
);

app.UseAuthentication();
app.UseAuthorization();

/// Пропускаем /api/auth/sign-in без авторизации
app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/api/auth/sign-in"),
    middlewareBuilder => middlewareBuilder.Use((context, next) => next())
);

// Кастомная авторизация по ролям и маршрутам
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();
    var method = context.Request.Method;

    if (!path.StartsWith("/api/"))
    {
        await next(context);
        return;
    }

    // Если пользователь не аутентифицирован
    if (!context.User.Identity!.IsAuthenticated)
    {
        if (path.StartsWith("/api/auth/sign-in"))
        {
            await next(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Вы не аутентифицированы");
        return;
    }
    
    // Register + Doctor (общие маршруты)
    if (context.User.IsInRole(Roles.Registrar.ToString()) || 
        context.User.IsInRole(Roles.Doctor.ToString()))
    {
        if ((path.StartsWith("/api/patient") && method == "GET") ||
            (path.StartsWith("/api/patient") && path.Contains("/") && method == "GET") ||
            (path.StartsWith("/api/appointment") && path.Contains("/") && method == "GET") ||
            (path.StartsWith("/api/visit") && path.Contains("/") && method == "GET") ||
            (path.StartsWith("/api/medicalrecord") && path.Contains("/") && method == "GET") ||
            (path.StartsWith("/api/transfer/senddata") && method == "GET"))
        {
            await next(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("Доступ запрещён. Требуется роль 'Doctor' или 'Registrar'");
        return;
    }

    // Admin: authservice - GET, POST, PUT
    if (path.StartsWith("/api/auth") && context.User.IsInRole(Roles.Admin.ToString()))
    {
        if (method == "GET" || (method == "POST" && path.Contains("sign-up")))
        {
            await next(context);
            return;
        }
    }

    // Doctor
    if (context.User.IsInRole(Roles.Doctor.ToString()))
    {
        if ((path.StartsWith("/api/appointment") && method == "GET" && path.Contains("/doctor/")) ||
            (path.StartsWith("/api/medicalrecord/medicalrecord") && method == "GET") ||
            (path.StartsWith("/api/medicalrecord") && method == "GET") ||
            (path.StartsWith("/api/visit") && method == "POST") ||
            (path.StartsWith("/api/medicalrecord") && path.Contains("/patient/") && method == "GET") ||
            (path.StartsWith("/api/auth/change-password") && method == "POST")
           )
        {
            await next(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("Доступ запрещён. Требуется роль 'Doctor'");
        return;
    }

    // Registrar
    if (context.User.IsInRole(Roles.Registrar.ToString()))
    {
        if ((path.StartsWith("/api/patients") && (method == "POST" || method == "PUT")) ||
            (path.StartsWith("/api/appointment") && (method == "GET" || method == "POST")) ||
            (path.StartsWith("/api/visit") && method == "GET") ||
            (path.StartsWith("/api/auth/change-password") && method == "POST")
           )
        {
            await next(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("Доступ запрещён. Требуется роль 'Registrar'");
        return;
    }

    // Admin
    if (context.User.IsInRole(Roles.Admin.ToString()))
    {
        if ((path.StartsWith("/api/auth") && (method == "GET" || method == "POST" || method == "PUT")) ||
            (path.StartsWith("/api/userprofile") && (method == "POST" || method == "PUT" || method == "GET")) ||
            (path.StartsWith("/api/transfer") && (method == "GET" || method == "POST")) ||
            (path.StartsWith("/api/auth/change-password") && method == "POST")
           )
        {
            await next(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("Доступ запрещён. Требуется роль 'Admin'");
        return;
    }

    // Все остальные запросы запрещены
    context.Response.StatusCode = StatusCodes.Status403Forbidden;
    await context.Response.WriteAsync("Нет прав для этого действия");
});


app.MapReverseProxy();
app.MapControllers();

app.Run();

public enum Roles
{
    Admin,
    Doctor,
    Registrar
}