using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;
using TransferService.Models;

namespace TransferService.SyncDataService;

public class AppointmentDataClient(IConfiguration configuration, IMapper mapper) : IAppointmentDataClient
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<Appointment>> GetAppointments()
    {
        var result = new List<Appointment>();
        var channel = GrpcChannel.ForAddress(_configuration["GrpcChannels:Appointment"]);
        var client = new GrpcAppointment.GrpcAppointmentClient(channel);
        var call = client.GetAllAppointmentsForTransfer();
        while (await call.ResponseStream.MoveNext())
        {
            var response = call.ResponseStream.Current;

            if (response.AppointmentId == -1)
            {
                Console.WriteLine("-->[INFO] All appointments received");
                await call.RequestStream.CompleteAsync();
                break;
            }

            Console.WriteLine($"-->[INFO] Grpc received {response.AppointmentId} {response.Status} {response.Reason}");
            result.Add(_mapper.Map<Appointment>(response));
            await call.RequestStream.WriteAsync(new GrpcAppointmentRequest()
            {
                AppointmentId = response.AppointmentId,
                IsTransferred = true
            });
        }

        Console.WriteLine("-->[INFO] Grpc data was received");
        return result;
    }
}