using System.Text.Json;
using AutoMapper;
using UserManagementService.Data;
using UserManagementService.Dtos;
using UserManagementService.Models;

namespace UserManagementService.EventProcessing;

public class EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper) : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IMapper _mapper = mapper;

    public void ProcessEvent(string message)
    {
        addUser(message);
    }

    private void addUser(string message)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IRepository>();
        var userReceived = JsonSerializer.Deserialize<UserReceiveDto>(message);
        try
        {
            var user = _mapper.Map<User>(userReceived);
            if (!repo.ExternalUserExists(userReceived.Id))
            {
                user.ExternalId = userReceived.Id;
                repo.CreateUser(user);
                repo.SaveChanges();
            }
            else
            {
                Console.WriteLine("-->[INFO] User already exists!");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] Something went wrong! {e}");
        }
    }
}