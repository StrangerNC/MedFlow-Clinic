using Microsoft.EntityFrameworkCore;
using TransferService.Data;
using TransferService.SyncDataService;

namespace TransferService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        //CustomDI
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddScoped<IRepository, Repository>();
        builder.Services.AddScoped<IAppointmentDataClient, AppointmentDataClient>();
        builder.Services.AddScoped<IMedicalRecordDataClient, MedicalRecordDataClient>();
        builder.Services.AddScoped<IPatientDataClient, PatientDataClient>();
        builder.Services.AddScoped<IUserManagementDataClient, UserManagementDataClient>();
        builder.Services.AddScoped<IGatherAndPutData, GatherAndPutData>();
        builder.Services.AddScoped<ISendDataClient, SendDataClient>();
        builder.Services.AddHostedService<ScheduledSendData>();
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


        app.Run();
    }
}