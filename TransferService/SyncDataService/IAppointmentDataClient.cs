using TransferService.Models;

namespace TransferService.SyncDataService;

public interface IAppointmentDataClient
{
    Task<IEnumerable<Appointment>> GetAppointments();
}