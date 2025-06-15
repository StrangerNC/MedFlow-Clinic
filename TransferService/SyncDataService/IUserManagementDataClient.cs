using TransferService.Models;

namespace TransferService.SyncDataService;

public interface IUserManagementDataClient
{
    Task<IEnumerable<UserProfile>> GetUserProfiles();
}