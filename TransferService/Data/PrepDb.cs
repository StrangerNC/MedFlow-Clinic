using Microsoft.EntityFrameworkCore;

namespace TransferService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        try
        {
            Console.WriteLine("-->[INFO] Attempting apply migrations...");
            context.Database.Migrate();
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] Migrations failed {e}");
        }
    }
}