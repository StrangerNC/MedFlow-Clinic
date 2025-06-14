using System.Text.Json;
using AppointmentService.Data;
using AppointmentService.Dtos;
using AppointmentService.Models;
using AutoMapper;

namespace AppointmentService.EventProcessing;

public class EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper) : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IMapper _mapper = mapper;

    public void ProcessEvent(string message, string type)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IRepository>();
        switch (type)
        {
            case "patient":
                AddPatient(message, repo);
                break;
            case "doctor":
                AddDoctor(message, repo);
                break;
        }
    }

    private void AddPatient(string message, IRepository repo)
    {
        //ToDo make ExternalId -> Id for a doctorpublisheddto also
        var patientRecieved = JsonSerializer.Deserialize<PatientReceivedDto>(message);
        try
        {
            var patient = _mapper.Map<Patient>(patientRecieved);
            if (!repo.PatientExists(patientRecieved.Id))
            {
                patient.ExternalId = patientRecieved.Id;
                repo.CreatePatient(patient);
                repo.SaveChanges();
            }
            else
            {
                Console.WriteLine("-->[INFO] Patient already exists!");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] Something went wrong: {e}");
        }
    }

    private void AddDoctor(string message, IRepository repo)
    {
        var doctorPublished = JsonSerializer.Deserialize<DoctorReceivedDto>(message);
        try
        {
            var doctor = _mapper.Map<Doctor>(doctorPublished);
            if (!repo.DoctorExists(doctorPublished.Id))
            {
                doctor.ExternalId = doctorPublished.Id;
                repo.CreateDoctor(doctor);
                repo.SaveChanges();
            }
            else
            {
                Console.WriteLine("-->[INFO] Doctor already exists!");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] Something went wrong: {e}");
        }
    }
}