using TransferService.Models;

namespace TransferService.Data;

public interface IRepository
{
    bool SaveChanges();

    //Appointment
    Task<Appointment?> GetAppointment(int externalId);
    Task<IEnumerable<Appointment>> GetAppointmentsToSend();
    void CreateAppointment(Appointment appointment);
    void UpdateAppointment(Appointment appointment);

    //ClinicData
    Task<ClinicData?> GetClinicData();
    void UpdateClinicData(ClinicData clinicData);

    //MedicalRecord
    Task<MedicalRecord?> GetMedicalRecord(int externalId);
    Task<IEnumerable<MedicalRecord>> GetMedicalRecordsToSend();
    void CreateMedicalRecord(MedicalRecord medicalRecord);
    void UpdateMedicalRecord(MedicalRecord medicalRecord);

    //Patient
    Task<Patient?> GetPatient(int externalId);
    Task<IEnumerable<Patient>> GetPatientsToSend();
    void CreatePatient(Patient patient);
    void UpdatePatient(Patient patient);

    //UserProfile
    Task<UserProfile?> GetUserProfile(int externalId);
    Task<IEnumerable<UserProfile>> GetUserProfilesToSend();
    void CreateUserProfile(UserProfile userProfile);
    void UpdateUserProfile(UserProfile userProfile);

    //Visit
    Task<Visit?> GetVisit(int externalId);
    Task<IEnumerable<Visit>> GetVisitsToSend();
    void CreateVisit(Visit visit);
    void UpdateVisit(Visit visit);
}