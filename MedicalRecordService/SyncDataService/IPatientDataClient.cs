using MedicalRecordService.Models;

namespace MedicalRecordService.SyncDataService;

public interface IPatientDataClient
{
    Task<IEnumerable<Patient>> GetAllPatients();
}