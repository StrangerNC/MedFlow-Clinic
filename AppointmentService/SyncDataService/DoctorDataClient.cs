using AppointmentService.Dtos;
using AppointmentService.Models;
using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;

namespace AppointmentService.SyncDataService;

public class DoctorDataClient(IConfiguration configuration, IMapper mapper) : IDoctorDataClient
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<Doctor>> GetAllDoctors()
    {
        var result = new List<Doctor>();
        var channel = GrpcChannel.ForAddress(_configuration["GrpcChannels:UserManagement"]);
        var client = new GrpcUserManagement.GrpcUserManagementClient(channel);
        var emptyRequest = new GrpcUserManagementEmpty();
        var call = client.GetAllUserProfiles(emptyRequest);
        while (await call.ResponseStream.MoveNext())
        {
            var response = call.ResponseStream.Current;
            if (response.UserProfileId == -1)
            {
                Console.WriteLine("-->[INFO] All doctors received");
                break;
            }

            Console.WriteLine($"-->[INFO] Grpc received {response.UserProfileId} {response.FullName}");
            var doctorPublished = _mapper.Map<DoctorPublishedDto>(response);
            var doctor = _mapper.Map<Doctor>(doctorPublished);
            result.Add(doctor);
        }

        Console.WriteLine("-->[INFO] Grpc doctors received");
        return result;
    }
}