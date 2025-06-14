using System.Linq.Expressions;
using AppointmentService.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentService.Data;

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

    public async Task<Appointment?> GetAppointment(int id)
    {
        var appointment = await context.Appointments.FirstOrDefaultAsync(p => p.Id == id);
        return appointment;
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentByDoctor(int doctorId)
    {
        var appointments = await context.Appointments.Where(p => p.DoctorId == doctorId).ToListAsync();
        return appointments;
    }

    public void CreateAppointment(Appointment appointment)
    {
        context.Appointments.Add(appointment);
    }

    public void UpdateAppointment(Appointment appointment)
    {
        context.Appointments.Update(appointment);
    }

    public void DeleteAppointment(Appointment appointment)
    {
        context.Appointments.Remove(appointment);
    }

    public async Task<IEnumerable<Appointment>> FindAppointment(Expression<Func<Appointment, bool>> predicate)
    {
        return await context.Appointments.Where(predicate).ToListAsync();
    }

    public void UpdateTransferredAppointmentStatus(int appointmentId, bool isTransferred)
    {
        var appointment = context.Appointments.FirstOrDefault(p => p.Id == appointmentId);
        if (appointment != null)
            appointment.IsTransferred = isTransferred;
    }

    public async Task<IEnumerable<Doctor>> GetDoctors()
    {
        var doctors = await context.Doctors.ToListAsync();
        return doctors;
    }

    public async Task<Doctor?> GetDoctor(int id)
    {
        var doctor = await context.Doctors.FirstOrDefaultAsync(p => p.ExternalId == id);
        return doctor;
    }

    public void CreateDoctor(Doctor doctor)
    {
        context.Doctors.Add(doctor);
    }

    public bool DoctorExists(int externalId)
    {
        return context.Doctors.Any(p => p.ExternalId == externalId);
    }

    public async Task<IEnumerable<Patient>> GetPatients()
    {
        var patients = await context.Patients.ToListAsync();
        return patients;
    }

    public async Task<Patient?> GetPatient(int id)
    {
        var patient = await context.Patients.FirstOrDefaultAsync(p => p.ExternalId == id);
        return patient;
    }

    public void CreatePatient(Patient patient)
    {
        context.Patients.Add(patient);
    }

    public bool PatientExists(int externalId)
    {
        return context.Patients.Any(p => p.ExternalId == externalId);
    }
}