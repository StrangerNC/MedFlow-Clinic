using Microsoft.EntityFrameworkCore;
using PatientService.AsyncDataService;
using PatientService.Data;
using RabbitMQ.Client.Events;

namespace PatientService;

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

        // Custom DI
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddScoped<IRepository, Repository>();
        builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
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
        app.MapGrpcService<SyncDataService.GrpcPatient>();
        app.MapControllers();

        app.Run();
    }
}