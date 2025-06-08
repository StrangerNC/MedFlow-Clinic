using UserManagementService.Models;

namespace UserManagementService.SyncDataService;

public interface IAuthDataClient
{
    Task<IEnumerable<User>> ReturnAllUsers();
}