using AutoMapper;
using MedicalRecordService.Data;
using MedicalRecordService.Dtos;
using MedicalRecordService.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalRecordService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MedicalRecordController(IRepository repository, IMapper mapper) : ControllerBase
{
    private readonly IRepository _repository = repository;
    private readonly IMapper _mapper = mapper;
    //ToDo make get functions returns external patient Id as patient Id
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicalRecordReadDto>>> GetMedicalRecords()
    {
        try
        {
            var medicalrecords = await _repository.GetMedicalRecords();
            return Ok(_mapper.Map<IEnumerable<MedicalRecordReadDto>>(medicalrecords));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] MedicalRecordController GetAllAppointments action {e}");
            return StatusCode(500);
        }
    }

    [HttpGet("patient/{id}")]
    public async Task<ActionResult<IEnumerable<MedicalRecordReadDto>>> GetPatientMedicalRecords(int id)
    {
        try
        {
            var medicalrecords = await _repository.GetMedicalRecordByPatient(id);
            return Ok(_mapper.Map<IEnumerable<MedicalRecordReadDto>>(medicalrecords));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] MedicalRecordController GetPatientMedicalRecords action {e}");
            return StatusCode(500);
        }
    }

    [HttpGet("{id}", Name = nameof(GetMedicalRecord))]
    public async Task<ActionResult<MedicalRecordReadDto>> GetMedicalRecord(int id)
    {
        try
        {
            var medicalrecord = await _repository.GetMedicalRecord(id);
            if (medicalrecord == null)
                return NotFound();
            return Ok(_mapper.Map<MedicalRecordReadDto>(medicalrecord));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] MedicalRecordController GetMedicalRecord action {e}");
            return StatusCode(500);
        }
    }

    [HttpPost]
    public async Task<ActionResult<MedicalRecordCreateDto>> CreateMedicalRecord(
        MedicalRecordCreateDto medicalRecordCreateDto)
    {
        try
        {
            var patient = await _repository.GetPatient(medicalRecordCreateDto.PatientId);
            if (patient == null)
                return NotFound();
            var medicalRecord = _mapper.Map<MedicalRecord>(medicalRecordCreateDto);
            _repository.CreateMedicalRecord(medicalRecord);
            _repository.SaveChanges();
            var medicalRecordReadDto = _mapper.Map<MedicalRecordReadDto>(medicalRecord);
            return CreatedAtAction(nameof(GetMedicalRecord), new { id = medicalRecordReadDto.Id },
                medicalRecordReadDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] MedicalRecordController CreateMedicalRecord action {e}");
            return StatusCode(500);
        }
    }
}