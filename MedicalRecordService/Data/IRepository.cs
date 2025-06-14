using MedicalRecordService.Models;

namespace MedicalRecordService.Data;

public interface IRepository
{
    bool SaveChanges();

    //Appointment
    Task<IEnumerable<Appointment>> GetAppointments();
    Task<Appointment?> GetAppointment(int externalId);
    void CreateAppointment(Appointment appointment);

    //MedicalRecord
    Task<IEnumerable<MedicalRecord>> GetMedicalRecords();
    Task<MedicalRecord?> GetMedicalRecord(int id);
    void CreateMedicalRecord(MedicalRecord medicalRecord);
    void UpdateMedicalRecord(MedicalRecord medicalRecord);
    void UpdateMedicalRecordTransferStatus(int id, bool isTransferred);

    //Patient
    Task<IEnumerable<Patient>> GetPatients();
    Task<Patient?> GetPatient(int externalId);
    void CreatePatient(Patient patient);

    //Visit
    Task<IEnumerable<Visit>> GetVisits();
    Task<Visit?> GetVisit(int id);
    void CreateVisit(Visit visit);
    void UpdateVisit(Visit visit);
    void UpdateVisitTransferStatus(int id, bool isTransferred);
}