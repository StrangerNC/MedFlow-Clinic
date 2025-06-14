using UserManagementService.Dtos;

namespace UserManagementService.AsyncDataService;

public interface IMessageBusClient
{
    Task PublishNewUserProfile(UserProfilePublishDto userProfile);
}