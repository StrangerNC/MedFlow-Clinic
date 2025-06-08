using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserManagementService.Models;
using UserManagementService.SyncDataService;

namespace UserManagementService.Data;

public class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var grpcClient = scope.ServiceProvider.GetRequiredService<IAuthDataClient>();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
        try
        {
            Console.WriteLine("-->[INFO] Database migrations started...");
            context.Database.Migrate();
        }
        catch (Exception e)
        {
            Console.WriteLine("-->[ERROR] Database migrations failed...");
        }

        SeedAuthData(grpcClient, repository);
    }

    public static void SeedAuthData(IAuthDataClient grpcClient, IRepository repository)
    {
        var users = grpcClient.ReturnAllUsers().Result;
        SeedData(repository, users);
    }

    private static void SeedData(IRepository repository, IEnumerable<User> users)
    {
        Console.WriteLine("-->[INFO] Seeding new users...");
        try
        {
            foreach (var user in users)
            {
                if (!repository.ExternalUserExists(user.ExternalId))
                    repository.CreateUser(user);
            }

            _ = repository.SaveChanges();
            Console.WriteLine("-->[INFO] Prep Population Complete");
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] Prep Population Failed exception {e}");
        }
    }
}