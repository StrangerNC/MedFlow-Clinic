using MedicalRecordService.SyncDataService;
using Microsoft.EntityFrameworkCore;

namespace MedicalRecordService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var patientsClient = serviceScope.ServiceProvider.GetRequiredService<IPatientDataClient>();
        var appointmentsClient = serviceScope.ServiceProvider.GetRequiredService<IAppointmentDataClient>();
        try
        {
            Console.WriteLine("-->[INFO] Attempting to apply migrations...");
            context.Database.Migrate();
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] Failed to apply migrations! {e}");
        }

        SeedData(context, appointmentsClient, patientsClient);
    }

    private static void SeedData(AppDbContext context, IAppointmentDataClient appointmentsClient,
        IPatientDataClient patientsClient)
    {
        try
        {
            Console.WriteLine("-->[INFO] Seeding data...");
            var patients = patientsClient.GetAllPatients().Result;
            var appointments = appointmentsClient.GetAllAppointments().Result;
            foreach (var patient in patients)
            {
                if (!context.Patients.Any(a => a.ExternalId == patient.Id))
                    context.Patients.Add(patient);
            }

            foreach (var appointment in appointments)
            {
                if (!context.Appointments.Any(a => a.ExternalId == appointment.Id))
                    context.Appointments.Add(appointment);
            }

            context.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] Seeding data failed! e: {e}");
        }
    }
}