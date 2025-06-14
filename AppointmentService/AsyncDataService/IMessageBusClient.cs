using AppointmentService.Dtos;
using AppointmentService.Models;

namespace AppointmentService.AsyncDataService;

public interface IMessageBusClient
{
    Task PublishNewAppointment(AppointmentPublishDto appointment);
}