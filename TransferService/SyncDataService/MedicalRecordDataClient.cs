using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;
using TransferService.Models;

namespace TransferService.SyncDataService;

public class MedicalRecordDataClient(IConfiguration configuration, IMapper mapper) : IMedicalRecordDataClient
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<MedicalRecord>> GetMedicalRecords()
    {
        var result = new List<MedicalRecord>();
        var channel = GrpcChannel.ForAddress(_configuration["GrpcChannels:MedicalRecord"]);
        var client = new GrpcMedicalRecord.GrpcMedicalRecordClient(channel);
        var call = client.GetMedicalRecordForTransfer();
        while (await call.ResponseStream.MoveNext())
        {
            var response = call.ResponseStream.Current;
            if (response.MedicalRecordId == -1)
            {
                Console.WriteLine("-->[INFO] All medical records received");
                await call.RequestStream.CompleteAsync();
                break;
            }

            Console.WriteLine($"-->[INFO] Grpc received {response.MedicalRecordId} {response.PatientId}");
            result.Add(_mapper.Map<MedicalRecord>(response));

            await call.RequestStream.WriteAsync(new GrpcMedicalRecordRequest()
            {
                MedicalRecordId = response.MedicalRecordId,
                IsTransferred = true
            });
        }

        Console.WriteLine("-->[INFO] Grpc data was received");
        return result;
    }

    public async Task<IEnumerable<Visit>> GetVisits()
    {
        var result = new List<Visit>();
        var channel = GrpcChannel.ForAddress(_configuration["GrpcChannels:MedicalRecord"]);
        var client = new GrpcMedicalRecord.GrpcMedicalRecordClient(channel);
        var call = client.GetVisitForTransfer();
        while (await call.ResponseStream.MoveNext())
        {
            var response = call.ResponseStream.Current;
            if (response.VisitId == -1)
            {
                Console.WriteLine("-->[INFO] All visits received");
                await call.RequestStream.CompleteAsync();
                break;
            }

            Console.WriteLine($"-->[INFO] Grpc received {response.VisitId} {response.ChiefComplaint}");
            result.Add(_mapper.Map<Visit>(response));

            await call.RequestStream.WriteAsync(new GrpcVisitRequest()
            {
                VisitId = response.VisitId,
                IsTransferred = true
            });
        }

        Console.WriteLine("-->[INFO] Grpc data was received");
        return result;
    }
}