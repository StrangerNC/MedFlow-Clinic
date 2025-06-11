using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PatientService.AsyncDataService;
using PatientService.Data;
using PatientService.Dtos;
using PatientService.Models;

namespace PatientService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientController(IRepository repository, IMapper mapper, IMessageBusClient messageBus) : ControllerBase
{
    private readonly IRepository _repository = repository;
    private readonly IMapper _mapper = mapper;
    private readonly IMessageBusClient _messageBusClient = messageBus;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientReadDto>>> GetPatients()
    {
        try
        {
            var patients = await _repository.GetPatients();
            return Ok(_mapper.Map<IEnumerable<PatientReadDto>>(patients));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] PatientController GetPatients action exception {e}");
            return StatusCode(500);
        }
    }

    [HttpGet("{id:int}", Name = nameof(GetPatient))]
    public async Task<ActionResult<PatientReadDto>> GetPatient(int id)
    {
        try
        {
            var patient = await _repository.GetPatient(id);
            if (patient == null)
                return NotFound();
            _repository.SaveChanges();
            return Ok(_mapper.Map<PatientReadDto>(patient));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] PatientController GetPatient action exception {e}");
            return StatusCode(500);
        }
    }

    [HttpPost]
    public async Task<ActionResult<PatientReadDto>> CreatePatient(PatientCreateDto patientCreateDto)
    {
        try
        {
            var isPatientExist = await _repository.GetPatients((patient =>
                patient.Passport == patientCreateDto.Passport));
            if (isPatientExist.Any())
                return BadRequest(409);
            var patient = _mapper.Map<Patient>(patientCreateDto);
            patient.DateOfBirth = Convert.ToDateTime(patientCreateDto.DateOfBirth).ToUniversalTime();
            _repository.CreatePatient(patient);
            _repository.SaveChanges();
            var patientReadDto = _mapper.Map<PatientReadDto>(patient);
            var patientPublishDto = _mapper.Map<PatientPublishDto>(patient);
            await _messageBusClient.PublishNewPatient(patientPublishDto);
            return CreatedAtRoute(nameof(GetPatient), new { id = patient.Id }, patientReadDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] PatientController CreatePatient action exception {e}");
            return BadRequest(500);
        }
    }


    //Todo search


    [HttpPut("{id:int}")]
    public async Task<ActionResult<PatientReadDto>> UpdatePatient(int id, PatientCreateDto patientCreateDto)
    {
        try
        {
            var patient = await _repository.GetPatient(id);
            if (patient == null)
                return NotFound();
            patient = _mapper.Map(patientCreateDto, patient);
            patient.DateOfBirth = Convert.ToDateTime(patientCreateDto.DateOfBirth).ToUniversalTime();
            _repository.UpdatePatient(patient);
            _repository.SaveChanges();
            var patientReadDto = _mapper.Map<PatientReadDto>(patient);
            var patientPublishDto = _mapper.Map<PatientPublishDto>(patient);
            await _messageBusClient.PublishNewPatient(patientPublishDto);
            return Ok(patientReadDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] PatientController UpdatePatient action exception {e}");
            return StatusCode(500);
        }
    }

    [HttpGet("search/{expression}")]
    public async Task<ActionResult<IEnumerable<PatientReadDto>>> SearchPatients(string expression)
    {
        try
        {
            var patients = await _repository.GetPatients((patient =>
                patient.Passport == expression || patient.FirstName == expression || patient.LastName == expression ||
                patient.MiddleName == expression));
            return Ok(_mapper.Map<IEnumerable<PatientReadDto>>(patients));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] PatientController SearchPatients action exception {e}");
            return StatusCode(500);
        }
    }
}