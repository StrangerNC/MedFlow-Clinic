using AppointmentService.AsyncDataService;
using AppointmentService.Data;
using AppointmentService.EventProcessing;
using AppointmentService.SyncDataService;
using Microsoft.EntityFrameworkCore;

namespace AppointmentService;

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
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddScoped<IRepository, Repository>();
        builder.Services.AddScoped<IDoctorDataClient, DoctorDataClient>();
        builder.Services.AddScoped<IPatientDataClient, PatientDataClient>();
        builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
        builder.Services.AddScoped<IMessageBusClient, MessageBusClient>();
        builder.Services.AddHostedService<DoctorMessageBusSubscriber>();
        builder.Services.AddHostedService<PatientMessageBusSubscriber>();

        var app = builder.Build();
        PrepDb.PrepPopulation(app);
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}