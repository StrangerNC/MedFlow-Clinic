using AppointmentService.Data;
using AppointmentService.Dtos;
using AppointmentService.Models;
using AutoMapper;
using Grpc.Core;

namespace AppointmentService.SyncDataService;

public class GrpcAppointment(IRepository repository, IMapper mapper)
    : AppointmentService.GrpcAppointment.GrpcAppointmentBase
{
    private readonly IRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public override async Task GetAllAppointments(GrpcAppointmentEmpty request,
        IServerStreamWriter<GrpcAppointmentResponse> responseStream, ServerCallContext context)
    {
        var appointments = await _repository.GetAppointments();
        foreach (var appointment in appointments)
        {
            var dto = _mapper.Map<AppointmentPublishDto>(appointment);
            await responseStream.WriteAsync(new GrpcAppointmentResponse()
            {
                AppointmentId = dto.Id,
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                Status = dto.Status,
                Reason = dto.Reason
            });
        }

        Console.WriteLine("-->[INFO] Grpc data was send");
        await Task.CompletedTask;
    }

    public override async Task GetAllAppointmentsForTransfer(IAsyncStreamReader<GrpcAppointmentRequest> requestStream,
        IServerStreamWriter<GrpcAppointmentResponse> responseStream,
        ServerCallContext context)
    {
        var appointments = await _repository.GetAppointments();
        var updateTask = new List<Task>();
        var updateAppointments = new List<GrpcAppointmentRequest>();
        foreach (var appointment in appointments)
        {
            var dto = _mapper.Map<AppointmentPublishDto>(appointment);
            await responseStream.WriteAsync(new GrpcAppointmentResponse()
            {
                AppointmentId = dto.Id,
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                Status = dto.Status,
                Reason = dto.Reason
            });
        }

        await responseStream.WriteAsync(new GrpcAppointmentResponse()
        {
            AppointmentId = -1,
            PatientId = -1,
            DoctorId = -1,
            Status = "",
            Reason = "",
        });
        await foreach (var msg in requestStream.ReadAllAsync())
        {
            updateTask.Add(Task.Run(() => { updateAppointments.Add(msg); }));
        }

        await Task.WhenAll(updateTask);
        foreach (var appointment in updateAppointments)
        {
            _repository.UpdateTransferredAppointmentStatus(appointment.AppointmentId, appointment.IsTransferred);
        }

        _repository.SaveChanges();
        Console.WriteLine("-->[INFO] Grpc data was send");

        await Task.CompletedTask;
    }
}