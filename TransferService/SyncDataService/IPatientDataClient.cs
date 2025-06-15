using TransferService.Models;

namespace TransferService.SyncDataService;

public interface IPatientDataClient
{
    Task<IEnumerable<Patient>> GetPatients();
}