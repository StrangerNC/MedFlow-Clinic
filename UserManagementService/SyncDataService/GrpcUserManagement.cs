using AutoMapper;
using Grpc.Core;
using UserManagementService.Data;
using UserManagementService.Dtos;
using UserManagementService.Models;

namespace UserManagementService.SyncDataService;

public class GrpcUserManagement(IRepository repository, IMapper mapper)
    : UserManagementService.GrpcUserManagement.GrpcUserManagementBase
{
    private readonly IRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public override async Task GetAllUserProfiles(IAsyncStreamReader<GrpcUserProfileRequest> requestStream,
        IServerStreamWriter<GrpcUserProfileResponse> responseStream,
        ServerCallContext context)
    {
        var userProfiles = await _repository.GetUserProfiles();
        var updateTasks = new List<Task>();
        var updateUserProfiles = new List<GrpcUserProfileRequest>();
        foreach (var userProfile in userProfiles)
        {
            var dto = _mapper.Map<UserProfilePublishDto>(userProfile);
            await responseStream.WriteAsync(new GrpcUserProfileResponse()
            {
                UserProfileId = dto.Id,
                FullName = dto.FullName,
                Department = dto.Department,
                Position = dto.Position
            });
        }

        await responseStream.WriteAsync(new GrpcUserProfileResponse()
        {
            UserProfileId = -1,
            FullName = "",
            Department = "",
            Position = ""
        });
        await foreach (var msg in requestStream.ReadAllAsync())
        {
            updateTasks.Add(Task.Run(() =>
            {
                updateUserProfiles.Add(msg);
            }));
        }
        await Task.WhenAll(updateTasks);
        foreach (var msg in updateUserProfiles)
        {
            _repository.UpdateTransferredUserProfileStatus(msg.UserProfileId, msg.IsTransferred);
        }
        _repository.SaveChanges();
        Console.WriteLine("-->[INFO] Grpc data was send");
    }
}