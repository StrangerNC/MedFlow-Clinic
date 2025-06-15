using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;
using MedicalRecordService.Dtos;
using MedicalRecordService.Models;

namespace MedicalRecordService.SyncDataService;

public class AppointmentDataClient(IConfiguration configuration, IMapper mapper) : IAppointmentDataClient
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<Appointment>> GetAllAppointments()
    {
        var result = new List<Appointment>();
        var channel = GrpcChannel.ForAddress(_configuration["GrpcChannels:Appointment"]);
        var client = new GrpcAppointment.GrpcAppointmentClient(channel);
        var emptyRequest = new GrpcAppointmentEmpty();
        var call = client.GetAllAppointments(emptyRequest);
        while (await call.ResponseStream.MoveNext())
        {
            var response = call.ResponseStream.Current;
            if (response.AppointmentId == -1)
            {
                Console.WriteLine("-->[INFO] All appointments received");
                break;
            }

            Console.WriteLine($"-->[INFO] Grpc received {response.AppointmentId} {response.PatientId}");
            var appointmentPublished = _mapper.Map<AppointmentPublishedDto>(response);
            appointmentPublished.ExternalId = response.AppointmentId;
            var appointment = _mapper.Map<Appointment>(appointmentPublished);
            result.Add(appointment);
        }

        Console.WriteLine("-->[INFO] Grpc appointments sent");
        return result;
    }
}