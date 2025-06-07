using AuthService.Models;
using AuthService.Utils;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

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
            Console.WriteLine($"-->[ERROR] Migration Failed! e: {e}");
        }

        if (!context.Users.Any())
        {
            Console.WriteLine("-->[INFO] Seeding Users...");
            var adminPassword = PasswordHasher.Hash("admin");
            var admin = new User()
            {
                UserName = "admin",
                PasswordHash = adminPassword.Item1,
                PasswordSalt = adminPassword.Item2,
                CreatedAt = DateTime.UtcNow,
                Role = Roles.Admin
            };
            context.Users.Add(admin);
            context.SaveChanges();
        }
    }
}