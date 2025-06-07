using AuthService.Data;
using AuthService.Dtos;
using AutoMapper;
using Grpc.Core;

namespace AuthService.SyncDataService;

public class GrpcAuth(IRepository repository, IMapper mapper) : AuthService.GrpcAuth.GrpcAuthBase
{
    private readonly IRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public override async Task GetAllUsers(
        IAsyncStreamReader<GrpcUserRequest> requestStream,
        IServerStreamWriter<GrpcUserResponse> responseStream,
        ServerCallContext context)
    {
        var users = await _repository.GetNotTransferredUsers();

        var updateTasks = new List<Task>();

        await foreach (var msg in requestStream.ReadAllAsync())
        {
            updateTasks.Add(Task.Run(() =>
            {
                _repository.UpdateTransferredUserStatus(msg.UserId, msg.IsTransferred);
            }));
        }

        await Task.WhenAll(updateTasks);
        _repository.SaveChanges();

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
    }
}