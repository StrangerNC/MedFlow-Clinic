using AutoMapper;
using MedicalRecordService.Data;
using MedicalRecordService.Dtos;
using MedicalRecordService.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalRecordService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VisitController(IRepository repository, IMapper mapper) : ControllerBase
{
    private readonly IRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<VisitReadDto>>> GetVisits()
    {
        try
        {
            var visits = await _repository.GetVisits();
            return Ok(_mapper.Map<IEnumerable<VisitReadDto>>(visits));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] VisitController GetVisits action {e}");
            return StatusCode(500);
        }
    }

    [HttpGet("medicalrecord/{id}")]
    public async Task<ActionResult<IEnumerable<VisitReadDto>>> GetVisitByMedicalRecord(int id)
    {
        try
        {
            var visits = await _repository.GetVisitByMedicalRecord(id);
            return Ok(_mapper.Map<IEnumerable<VisitReadDto>>(visits));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] VisitController GetVisitByMedicalRecord action {e}");
            return StatusCode(500);
        }
    }

    [HttpGet("{id}", Name = nameof(GetVisit))]
    public async Task<ActionResult<VisitReadDto>> GetVisit(int id)
    {
        try
        {
            var medicalRecord = await _repository.GetVisit(id);
            if (medicalRecord == null)
                return NotFound();
            return Ok(_mapper.Map<VisitReadDto>(medicalRecord));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] VisitController GetVisits action {e}");
            return StatusCode(500);
        }
    }

    [HttpPost]
    public async Task<ActionResult<VisitReadDto>> CreateVisit(VisitCreateDto visitCreateDto)
    {
        try
        {
            var appointment = await _repository.GetAppointment(visitCreateDto.AppointmentId);
            var medicalRecord = await _repository.GetMedicalRecord(visitCreateDto.MedicalRecordId);
            if (appointment == null || medicalRecord == null)
                return NotFound();
            var visit = _mapper.Map<Visit>(visitCreateDto);
            _repository.CreateVisit(visit);
            _repository.SaveChanges();
            var visitReadDto = _mapper.Map<VisitReadDto>(visit);
            return CreatedAtRoute(nameof(GetVisit), new { id = visitReadDto.Id }, visitReadDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] VisitController CreateVisit action {e}");
            return StatusCode(500);
        }
    }
}