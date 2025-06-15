using MedicalRecordService.Data;
using MedicalRecordService.SyncDataService;
using Microsoft.EntityFrameworkCore;

namespace MedicalRecordService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        //Custom DI
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddScoped<IRepository, Repository>();
        builder.Services.AddScoped<IPatientDataClient, PatientDataClient>();
        builder.Services.AddScoped<IAppointmentDataClient, AppointmentDataClient>();
        builder.Services.AddGrpc();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

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
        app.MapGrpcService<SyncDataService.GrpcMedicalRecord>();

        app.Run();
    }
}