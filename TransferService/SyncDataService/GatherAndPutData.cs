using TransferService.Data;

namespace TransferService.SyncDataService;

public class GatherAndPutData(
    IRepository repository,
    IAppointmentDataClient appointmentDataClient,
    IMedicalRecordDataClient medicalRecordDataClient,
    IPatientDataClient patientDataClient,
    IUserManagementDataClient userManagementDataClient):IGatherAndPutData
{
    private readonly IRepository _repository = repository;
    private readonly IAppointmentDataClient _appointmentDataClient = appointmentDataClient;
    private IMedicalRecordDataClient medicalRecordDataClient = medicalRecordDataClient;
    private IPatientDataClient patientDataClient = patientDataClient;
    private IUserManagementDataClient userManagementDataClient = userManagementDataClient;

    public async Task GetAndPutData()
    {
        try
        {
            Console.WriteLine("-->[INFO] Gathering and putting data...");
            var appointments = await _appointmentDataClient.GetAppointments();
            var medicalRecords = await medicalRecordDataClient.GetMedicalRecords();
            var visits = await medicalRecordDataClient.GetVisits();
            var userProfiles = await userManagementDataClient.GetUserProfiles();
            var patients = await patientDataClient.GetPatients();
            foreach (var appointment in appointments)
            {
                if (await _repository.GetAppointment(appointment.ExternalId) == null)
                    _repository.CreateAppointment(appointment);
            }

            foreach (var medicalRecord in medicalRecords)
            {
                if (await _repository.GetMedicalRecord(medicalRecord.ExternalId) == null)
                    _repository.CreateMedicalRecord(medicalRecord);
            }

            foreach (var visit in visits)
            {
                if (await _repository.GetVisit(visit.ExternalId) == null)
                    _repository.CreateVisit(visit);
            }

            foreach (var userProfile in userProfiles)
            {
                if (await _repository.GetUserProfile(userProfile.ExternalId) == null)
                    _repository.CreateUserProfile(userProfile);
            }

            foreach (var patient in patients)
            {
                if (await _repository.GetPatient(patient.ExternalId) == null)
                    _repository.CreatePatient(patient);
            }

            _repository.SaveChanges();
            Console.WriteLine("-->[INFO] GatherAndPutData succeed");
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] GetAndPutData {e}");
        }
    }
}