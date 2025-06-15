using MedicalRecordService.Models;

namespace MedicalRecordService.SyncDataService;

public interface IAppointmentDataClient
{
    Task<IEnumerable<Appointment>> GetAllAppointments();
}