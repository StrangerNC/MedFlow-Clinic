using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PatientService.Models;

namespace PatientService.Data;

public class Repository(AppDbContext context) : IRepository
{
    public bool SaveChanges()
    {
        return context.SaveChanges() > 0;
    }

    public void CreatePatient(Patient patient)
    {
        context.Patients.Add(patient);
    }

    public async Task<IEnumerable<Patient>> GetPatients()
    {
        return await context.Patients.ToListAsync();
    }

    public async Task<Patient?> GetPatient(int id)
    {
        return await context.Patients.FirstOrDefaultAsync(p => p.Id == id);
    }

    public void UpdatePatient(Patient patient)
    {
        context.Patients.Update(patient);
    }

    public async Task<IEnumerable<Patient>> GetPatients(Expression<Func<Patient, bool>> predicate)
    {
        return await context.Patients.Where(predicate).ToListAsync();
    }

    public void UpdateTrasferredPatientStatus(int patientId, bool isTransferred)
    {
        var patient = context.Patients.FirstOrDefault(p => p.Id == patientId);
        patient.IsTransferred = isTransferred;
    }
}