using MedicalRecordService.Dtos;
using MedicalRecordService.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalRecordService.Data;

public class Repository(AppDbContext context) : IRepository
{
    public bool SaveChanges()
    {
        return context.SaveChanges() > 0;
    }

    public async Task<IEnumerable<Appointment>> GetAppointments()
    {
        var appointments = await context.Appointments.ToListAsync();
        return appointments;
    }

    public async Task<Appointment?> GetAppointment(int externalId)
    {
        var appointment = await context.Appointments.FirstOrDefaultAsync(x => x.ExternalId == externalId);
        return appointment;
    }

    public void CreateAppointment(Appointment appointment)
    {
        context.Appointments.Add(appointment);
    }

    public async Task<IEnumerable<MedicalRecord>> GetMedicalRecords()
    {
        var medicalRecords = await context.MedicalRecords.Include(x => x.Patient).ToListAsync();
        return medicalRecords;
    }

    public async Task<IEnumerable<MedicalRecord>> GetMedicalRecordByPatient(int id)
    {
        return await context.MedicalRecords.Include(x => x.Patient).Where(x => x.PatientId == id).ToListAsync();
    }

    public async Task<MedicalRecord?> GetMedicalRecord(int id)
    {
        var medicalRecord = await context.MedicalRecords.Include(x => x.Patient).FirstOrDefaultAsync(x => x.Id == id);
        return medicalRecord;
    }

    public void CreateMedicalRecord(MedicalRecord medicalRecord)
    {
        context.MedicalRecords.Add(medicalRecord);
    }

    public void UpdateMedicalRecord(MedicalRecord medicalRecord)
    {
        context.MedicalRecords.Update(medicalRecord);
    }

    public async Task<IEnumerable<Patient>> GetPatients()
    {
        var patients = await context.Patients.ToListAsync();
        return patients;
    }

    public async Task<Patient?> GetPatient(int externalId)
    {
        var patient = await context.Patients.FirstOrDefaultAsync(x => x.ExternalId == externalId);
        return patient;
    }

    public void CreatePatient(Patient patient)
    {
        context.Patients.Add(patient);
    }

    public async Task<IEnumerable<Visit>> GetVisits()
    {
        var visits = await context.Visits.Include(x => x.Appointment).ToListAsync();
        return visits;
    }

    public async Task<IEnumerable<Visit>> GetVisitByMedicalRecord(int id)
    {
        return await context.Visits.Include(x => x.Appointment).Where(x => x.MedicalRecordId == id).ToListAsync();
    }

    public async Task<Visit?> GetVisit(int id)
    {
        var visit = await context.Visits.Include(x => x.Appointment).FirstOrDefaultAsync(x => x.Id == id);
        return visit;
    }

    public void CreateVisit(Visit visit)
    {
        context.Visits.Add(visit);
    }

    public void UpdateVisit(Visit visit)
    {
        context.Visits.Update(visit);
    }

    public void UpdateMedicalRecordTransferStatus(int id, bool isTransferred)
    {
        var medicalRecord = context.MedicalRecords.FirstOrDefault(x => x.Id == id);
        if (medicalRecord != null)
            medicalRecord.IsTransferred = isTransferred;
    }

    public void UpdateVisitTransferStatus(int id, bool isTransferred)
    {
        var visit = context.Visits.FirstOrDefault(x => x.Id == id);
        if (visit != null)
            visit.IsTransferred = isTransferred;
    }
}