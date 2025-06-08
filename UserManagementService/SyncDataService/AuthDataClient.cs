using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;
using UserManagementService.Dtos;
using UserManagementService.Models;

namespace UserManagementService.SyncDataService;

public class AuthDataClient(IConfiguration configuration, IMapper mapper) : IAuthDataClient
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<User>> ReturnAllUsers()
    {
        var result = new List<User>();
        var channel = GrpcChannel.ForAddress(_configuration["GrpcChannels:AuthService"]);
        var client = new GrpcAuth.GrpcAuthClient(channel);
        var call = client.GetAllUsers();
        var dummyRequest = new GrpcUserRequest();
        while (await call.ResponseStream.MoveNext())
        {
            var response = call.ResponseStream.Current;

            if (response.UserId == -1)
            {
                Console.WriteLine("-->[INFO] All users received");
                await call.RequestStream.CompleteAsync();
                break;
            }

            Console.WriteLine($"-->[INFO] Grpc received {response.UserId} {response.Role} {response.UserName}");
            var userPublished = mapper.Map<UserPublishedDto>(response);
            var user = _mapper.Map<User>(userPublished);
            result.Add(user);
            await call.RequestStream.WriteAsync(new GrpcUserRequest()
            {
                UserId = response.UserId,
                IsTransferred = true
            });
        }

        Console.WriteLine("-->[INFO] Grpc data was received");
        return result;
    }
}