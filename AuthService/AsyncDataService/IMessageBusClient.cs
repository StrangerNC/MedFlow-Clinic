using AuthService.Dtos;

namespace AuthService.AsyncDataService;

public interface IMessageBusClient
{
    //ToDo
    Task PublishNewUser(UserPublishDto user);
}