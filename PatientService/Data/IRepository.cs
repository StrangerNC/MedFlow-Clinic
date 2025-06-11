using System.Linq.Expressions;
using PatientService.Models;

namespace PatientService.Data;

public interface IRepository
{
    bool SaveChanges();
    void CreatePatient(Patient patient);
    Task<IEnumerable<Patient>> GetPatients();
    Task<Patient?> GetPatient(int id);

    void UpdatePatient(Patient patient);

    //Experimental function for search by some property
    Task<IEnumerable<Patient>> GetPatients(Expression<Func<Patient, bool>> predicate);
    void UpdateTrasferredPatientStatus(int patientId, bool isTransferred);
}