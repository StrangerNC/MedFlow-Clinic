using Microsoft.EntityFrameworkCore;
using UserManagementService.AsyncDataService;
using UserManagementService.Data;
using UserManagementService.EventProcessing;
using UserManagementService.SyncDataService;

namespace UserManagementService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        //Custom DI
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddScoped<IRepository, Repository>();
        builder.Services.AddScoped<IAuthDataClient, AuthDataClient>();
        builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
        builder.Services.AddHostedService<AuthMessageBusSubscriber>();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddGrpc();

        var app = builder.Build();

        PrepDb.PrepPopulation(app);
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.MapGrpcService<SyncDataService.GrpcUserManagement>();
        app.MapControllers();
        app.Run();
    }
}