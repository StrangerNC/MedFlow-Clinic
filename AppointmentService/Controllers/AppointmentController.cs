using AppointmentService.AsyncDataService;
using AppointmentService.Data;
using AppointmentService.Dtos;
using AppointmentService.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentController(IRepository repository, IMapper mapper, IMessageBusClient messageBus)
    : ControllerBase
{
    private readonly IRepository _repository = repository;
    private readonly IMapper _mapper = mapper;
    private readonly IMessageBusClient _messageBusClient = messageBus;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentReadDto>>> GetAppointments()
    {
        try
        {
            var appointments = await _repository.GetAppointments();
            return Ok(_mapper.Map<IEnumerable<AppointmentReadDto>>(appointments));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] AppointmentController GetAppointments action {e}");
            return StatusCode(500);
        }
    }

    [HttpGet("{id}", Name = nameof(GetAppointment))]
    public async Task<ActionResult<AppointmentReadDto>> GetAppointment(int id)
    {
        try
        {
            var appointment = await _repository.GetAppointment(id);
            return Ok(_mapper.Map<AppointmentReadDto>(appointment));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] AppointmentController GetAppointment action {e}");
            return StatusCode(500);
        }
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentReadDto>> CreateAppointment(AppointmentCreateDto appointmentCreateDto)
    {
        try
        {
            var doctor = await _repository.GetDoctor(appointmentCreateDto.DoctorExternalId);
            var patient = await _repository.GetPatient(appointmentCreateDto.PatientExternalId);
            if (doctor == null || patient == null)
                return NotFound();
            var appointment = _mapper.Map<Appointment>(appointmentCreateDto);
            appointment.PatientId = patient.Id;
            appointment.DoctorId = doctor.Id;
            appointment.AppointmentDate = Convert.ToDateTime(appointmentCreateDto.AppointmentDate).ToUniversalTime();
            _repository.CreateAppointment(appointment);
            _repository.SaveChanges();
            var appointmentReadDto = _mapper.Map<AppointmentReadDto>(appointment);
            var appointmentPublish = _mapper.Map<AppointmentPublishDto>(appointment);
            await _messageBusClient.PublishNewAppointment(appointmentPublish);
            return CreatedAtRoute(nameof(GetAppointment), new { Id = appointment.Id }, appointmentReadDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] AppointmentController CreateAppointment action {e}");
            return StatusCode(500);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AppointmentReadDto>> UpdateAppointment(int id,
        AppointmentCreateDto appointmentCreateDto)
    {
        try
        {
            var appointment = await _repository.GetAppointment(id);
            if (appointment == null)
                return NotFound();
            if (appointment.AppointmentDate != appointmentCreateDto.AppointmentDate)
                appointment.Status = Statuses.Resheduled;
            appointment.AppointmentDate = Convert.ToDateTime(appointmentCreateDto.AppointmentDate).ToUniversalTime();
            appointment.UpdatedAt = DateTime.UtcNow;
            _repository.UpdateAppointment(appointment);
            _repository.SaveChanges();
            var appointmentReadDto = _mapper.Map<AppointmentReadDto>(appointment);
            return CreatedAtRoute(nameof(GetAppointment), new { Id = appointment.Id }, appointmentReadDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] AppointmentController UpdateAppointment action {e}");
            return StatusCode(500);
        }
    }

    [HttpGet("search/{expression}")]
    public async Task<ActionResult<IEnumerable<AppointmentReadDto>>> FindAppointment(string expression)
    {
        try
        {
            var appointments =
                await _repository.FindAppointment(x => x.Reason == expression);
            return Ok(_mapper.Map<IEnumerable<AppointmentReadDto>>(appointments));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] AppointmentController findAppointment action {e}");
            return StatusCode(500);
        }
    }

    [HttpGet("patient/{id}")]
    public async Task<ActionResult<IEnumerable<AppointmentReadDto>>> GetAppointmentByPatient(int id)
    {
        try
        {
            var appointment = await _repository.GetAppointmentByPatient(id);
            return Ok(_mapper.Map<IEnumerable<AppointmentReadDto>>(appointment));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] AppointmentController GetAppointmentByPatient action {e}");
            return StatusCode(500);
        }
    }

    [HttpGet("doctor/{id}")]
    public async Task<ActionResult<IEnumerable<AppointmentReadDto>>> GetAppointmentByDoctor(int id)
    {
        try
        {
            var appointment = await _repository.GetAppointmentByDoctor(id);
            return Ok(_mapper.Map<IEnumerable<AppointmentReadDto>>(appointment));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] AppointmentController GetAppointmentByDoctor action {e}");
            return StatusCode(500);
        }
    }
}