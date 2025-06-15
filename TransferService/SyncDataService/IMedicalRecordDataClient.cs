using TransferService.Models;

namespace TransferService.SyncDataService;

public interface IMedicalRecordDataClient
{
    Task<IEnumerable<MedicalRecord>> GetMedicalRecords();
    Task<IEnumerable<Visit>> GetVisits();
}