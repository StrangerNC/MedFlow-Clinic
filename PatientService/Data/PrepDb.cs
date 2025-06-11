using Microsoft.EntityFrameworkCore;

namespace PatientService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        SeedData(context);
    }

    private static void SeedData(AppDbContext context)
    {
        try
        {
            Console.WriteLine("-->[INFO] Attempting to apply migrations...");
            context.Database.Migrate();
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] Migration Failed! {e}");
        }
    }
}