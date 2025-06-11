using PatientService.Dtos;

namespace PatientService.AsyncDataService;

public interface IMessageBusClient
{
    Task PublishNewPatient(PatientPublishDto patient);
}