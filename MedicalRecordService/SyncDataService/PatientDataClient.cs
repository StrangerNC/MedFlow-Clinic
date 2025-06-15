using MedicalRecordService.Dtos;
using MedicalRecordService.Models;
using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;

namespace MedicalRecordService.SyncDataService;

public class PatientDataClient(IConfiguration configuration, IMapper mapper) : IPatientDataClient
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<Patient>> GetAllPatients()
    {
        var result = new List<Patient>();
        var channel = GrpcChannel.ForAddress(_configuration["GrpcChannels:Patient"]);
        var client = new GrpcPatient.GrpcPatientClient(channel);
        var emptyRequest = new GrpcEmptyRequest();
        var call = client.GetAllPatients(emptyRequest);
        while (await call.ResponseStream.MoveNext())
        {
            var response = call.ResponseStream.Current;
            if (response.PatientId == -1)
            {
                Console.WriteLine("-->[INFO] All patients received");
                break;
            }

            Console.WriteLine($"-->[INFO] Grpc received {response.PatientId} {response.FirstName}");
            var patientPublished = _mapper.Map<PatientPublishedDto>(response);
            patientPublished.ExternalId = response.PatientId;
            var patient = _mapper.Map<Patient>(patientPublished);
            result.Add(patient);
        }

        Console.WriteLine("-->[INFO] Grpc patients received");
        return result;
    }
}