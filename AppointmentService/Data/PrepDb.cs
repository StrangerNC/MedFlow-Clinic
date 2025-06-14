using AppointmentService.SyncDataService;
using Microsoft.EntityFrameworkCore;

namespace AppointmentService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var doctorsClient = serviceScope.ServiceProvider.GetRequiredService<IDoctorDataClient>();
        var patientsClient = serviceScope.ServiceProvider.GetRequiredService<IPatientDataClient>();
        try
        {
            Console.WriteLine("-->[INFO] Attempting to apply migrations...");
            context.Database.Migrate();
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] Migration Failed! e: {e}");
        }

        SeedData(context, doctorsClient, patientsClient);
    }

    private static void SeedData(AppDbContext context, IDoctorDataClient doctorsClient,
        IPatientDataClient patientsClient)
    {
        try
        {
            Console.WriteLine("-->[INFO] Seeding data...");
            var doctors = doctorsClient.GetAllDoctors().Result;
            var patients = patientsClient.GetAllPatients().Result;
            foreach (var doctor in doctors)
            {
                if (!context.Doctors.Any(p => p.ExternalId == doctor.ExternalId))
                {
                    context.Doctors.Add(doctor);
                }
            }

            foreach (var patient in patients)
            {
                if (!context.Patients.Any(p => p.ExternalId == patient.ExternalId))
                {
                    context.Patients.Add(patient);
                }
            }

            context.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] Seeding data failed! e: {e}");
        }
    }
}