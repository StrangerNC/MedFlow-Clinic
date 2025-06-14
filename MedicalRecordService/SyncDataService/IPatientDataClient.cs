using AppointmentService.Models;

namespace AppointmentService.SyncDataService;

public interface IPatientDataClient
{
    Task<IEnumerable<Patient>> GetAllPatients();
}