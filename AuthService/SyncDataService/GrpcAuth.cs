using AuthService.Data;
using AuthService.Dtos;
using AuthService.Models;
using AutoMapper;
using Grpc.Core;

namespace AuthService.SyncDataService;

public class GrpcAuth(IRepository repository, IMapper mapper) : AuthService.GrpcAuth.GrpcAuthBase
{
    private readonly IRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public override async Task GetAllUsers(IAsyncStreamReader<GrpcUserRequest> requestStream,
        IServerStreamWriter<GrpcUserResponse> responseStream, ServerCallContext context)
    {
        var users = await _repository.GetNotTransferredUsers();
        var updateTasks = new List<Task>();
        var updateUsers = new List<GrpcUserRequest>();

        foreach (var user in users)
        {
            var dto = _mapper.Map<UserPublishDto>(user);
            await responseStream.WriteAsync(new GrpcUserResponse
            {
                UserId = dto.Id,
                UserName = dto.UserName,
                Role = dto.Role,
            });
        }

        await responseStream.WriteAsync(new GrpcUserResponse
        {
            UserId = -1,
            UserName = "",
            Role = "",
        });
        await foreach (var msg in requestStream.ReadAllAsync())
        {
            updateTasks.Add(Task.Run(() =>
            {
                // _repository.UpdateTransferredUserStatus(msg.UserId, msg.IsTransferred);
                updateUsers.Add(msg);
            }));
        }

        await Task.WhenAll(updateTasks);
        foreach (var user in updateUsers)
        {
            _repository.UpdateTransferredUserStatus(user.UserId,user.IsTransferred);
        }

        _repository.SaveChanges();
        Console.WriteLine("-->[INFO] Grpc data was send");
    }
}