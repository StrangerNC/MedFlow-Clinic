using AppointmentService.Models;

namespace AppointmentService.SyncDataService;

public interface IDoctorDataClient
{
    Task<IEnumerable<Doctor>> GetAllDoctors();
}