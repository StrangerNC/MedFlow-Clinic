using AuthService.AsyncDataService;
using AuthService.Data;
using AuthService.Utils;
using Microsoft.EntityFrameworkCore;

namespace AuthService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        //My custom di
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddScoped<IRepository, Repository>();
        builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
        builder.Services.AddGrpc();

        var app = builder.Build();
        //PrepDb
        PrepDb.PrepPopulation(app);
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.MapControllers();
        app.UseAuthorization();
        app.UseAuthentication();

        app.Run();
    }
}