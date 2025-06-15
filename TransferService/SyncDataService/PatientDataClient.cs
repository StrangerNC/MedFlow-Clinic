using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;
using TransferService.Models;

namespace TransferService.SyncDataService;

public class PatientDataClient(IConfiguration configuration, IMapper mapper) : IPatientDataClient
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<Patient>> GetPatients()
    {
        var result = new List<Patient>();
        var channel = GrpcChannel.ForAddress(_configuration["GrpcChannels:Patient"]);
        var client = new GrpcPatient.GrpcPatientClient(channel);
        var call = client.GetAllPatientsForTransfer();
        while (await call.ResponseStream.MoveNext())
        {
            var response = call.ResponseStream.Current;

            if (response.PatientId == -1)
            {
                Console.WriteLine("-->[INFO] All patients received");
                await call.RequestStream.CompleteAsync();
                break;
            }

            Console.WriteLine($"-->[INFO] Grpc received {response.PatientId} {response.FirstName} {response.LastName}");
            result.Add(_mapper.Map<Patient>(response));
            await call.RequestStream.WriteAsync(new GrpcPatientRequest()
            {
                PatientId = response.PatientId,
                IsTransferred = true
            });
        }

        Console.WriteLine("-->[INFO] Grpc data was received");
        return result;
    }
}