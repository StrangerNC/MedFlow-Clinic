using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;
using TransferService.Models;

namespace TransferService.SyncDataService;

public class UserManagementDataClient(IConfiguration configuration, IMapper mapper) : IUserManagementDataClient
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<UserProfile>> GetUserProfiles()
    {
        var result = new List<UserProfile>();
        var channel = GrpcChannel.ForAddress(_configuration["GrpcChannels:UserManagement"]);
        var client = new GrpcUserManagement.GrpcUserManagementClient(channel);
        var call = client.GetAllUserProfilesForTransfer();
        while (await call.ResponseStream.MoveNext())
        {
            var response = call.ResponseStream.Current;

            if (response.UserProfileId == -1)
            {
                Console.WriteLine("-->[INFO] All user profiles received");
                await call.RequestStream.CompleteAsync();
                break;
            }

            Console.WriteLine(
                $"-->[INFO] Grpc received {response.UserProfileId} {response.Position} {response.FullName}");
            result.Add(_mapper.Map<UserProfile>(response));
            await call.RequestStream.WriteAsync(new GrpcUserProfileRequest()
            {
                UserProfileId = response.UserProfileId,
                IsTransferred = true
            });
        }

        Console.WriteLine("-->[INFO] Grpc data was received");
        return result;
    }
}