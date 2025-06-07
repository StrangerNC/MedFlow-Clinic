using AuthService.Dtos;

namespace AuthService.AsyncDataService;

public interface IMessageBusClient
{
    Task PublishNewUser(UserPublishDto user);
}