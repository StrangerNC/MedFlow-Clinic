using System.Linq.Expressions;
using AppointmentService.Models;

namespace AppointmentService.Data;

public interface IRepository
{
    bool SaveChanges();

    //Appointment
    Task<IEnumerable<Appointment>> GetAppointments();
    Task<Appointment?> GetAppointment(int id);
    Task<IEnumerable<Appointment>> GetAppointmentByDoctor(int doctorId);
    void CreateAppointment(Appointment appointment);
    void UpdateAppointment(Appointment appointment);
    void DeleteAppointment(Appointment appointment);
    Task<IEnumerable<Appointment>> FindAppointment(Expression<Func<Appointment, bool>> predicate);
    void UpdateTransferredAppointmentStatus(int appointmentId, bool isTransferred);

    //Doctor
    Task<IEnumerable<Doctor>> GetDoctors();
    Task<Doctor?> GetDoctor(int id);
    void CreateDoctor(Doctor doctor);
    bool DoctorExists(int externalId);

    //Patient
    Task<IEnumerable<Patient>> GetPatients();
    Task<Patient?> GetPatient(int id);
    void CreatePatient(Patient patient);
    bool PatientExists(int externalId);
}