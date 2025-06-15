using Microsoft.EntityFrameworkCore;
using TransferService.Models;

namespace TransferService.Data;

public class Repository(AppDbContext context) : IRepository
{
    public bool SaveChanges()
    {
        return context.SaveChanges() > 0;
    }

    public async Task<Appointment?> GetAppointment(int externalId)
    {
        return await context.Appointments.FirstOrDefaultAsync(x => x.ExternalId == externalId);
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsToSend()
    {
        return await context.Appointments.Where(x => x.IsSent == false).ToListAsync();
    }

    public void CreateAppointment(Appointment appointment)
    {
        context.Appointments.Add(appointment);
    }

    public void UpdateAppointment(Appointment appointment)
    {
        context.Appointments.Update(appointment);
    }

    public async Task<ClinicData?> GetClinicData()
    {
        return await context.ClinicData.FirstOrDefaultAsync();
    }

    public void UpdateClinicData(ClinicData clinicData)
    {
        var clinic = context.ClinicData.FirstOrDefault();
        if (clinic == null)
            context.ClinicData.Add(clinicData);
        else
            context.ClinicData.Update(clinic);
    }

    public async Task<MedicalRecord?> GetMedicalRecord(int externalId)
    {
        return await context.MedicalRecords.FirstOrDefaultAsync(x => x.ExternalId == externalId);
    }

    public async Task<IEnumerable<MedicalRecord>> GetMedicalRecordsToSend()
    {
        return await context.MedicalRecords.Where(x => x.IsSent == false).ToListAsync();
    }

    public void CreateMedicalRecord(MedicalRecord medicalRecord)
    {
        context.MedicalRecords.Add(medicalRecord);
    }

    public void UpdateMedicalRecord(MedicalRecord medicalRecord)
    {
        context.MedicalRecords.Update(medicalRecord);
    }

    public async Task<Patient?> GetPatient(int externalId)
    {
        return await context.Patients.FirstOrDefaultAsync(x => x.ExternalId == externalId);
    }

    public async Task<IEnumerable<Patient>> GetPatientsToSend()
    {
        return await context.Patients.Where(x => x.IsSent == false).ToListAsync();
    }

    public void CreatePatient(Patient patient)
    {
        context.Patients.Add(patient);
    }

    public void UpdatePatient(Patient patient)
    {
        context.Patients.Update(patient);
    }

    public async Task<UserProfile?> GetUserProfile(int externalId)
    {
        return await context.UserProfiles.FirstOrDefaultAsync(x => x.ExternalId == externalId);
    }

    public async Task<IEnumerable<UserProfile>> GetUserProfilesToSend()
    {
        return await context.UserProfiles.Where(x => x.IsSent == false).ToListAsync();
    }

    public void CreateUserProfile(UserProfile userProfile)
    {
        context.UserProfiles.Add(userProfile);
    }

    public void UpdateUserProfile(UserProfile userProfile)
    {
        context.UserProfiles.Update(userProfile);
    }

    public async Task<Visit?> GetVisit(int externalId)
    {
        return await context.Visits.FirstOrDefaultAsync(x => x.ExternalId == externalId);
    }

    public async Task<IEnumerable<Visit>> GetVisitsToSend()
    {
        return await context.Visits.Where(x => x.IsSent == false).ToListAsync();
    }

    public void CreateVisit(Visit visit)
    {
        context.Visits.Add(visit);
    }

    public void UpdateVisit(Visit visit)
    {
        context.Visits.Update(visit);
    }
}