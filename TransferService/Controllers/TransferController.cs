using System.Runtime.InteropServices.JavaScript;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TransferService.Data;
using TransferService.Models;
using TransferService.SyncDataService;

namespace TransferService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransferController(IRepository repository, IGatherAndPutData gatherAndPut, ISendDataClient sendData)
    : ControllerBase
{
    private readonly IRepository _repository = repository;
    private readonly IGatherAndPutData gather = gatherAndPut;
    private readonly ISendDataClient send = sendData;

    [HttpGet(Name = nameof(GetClinicData))]
    public async Task<ActionResult<ClinicData>> GetClinicData()
    {
        try
        {
            var clinicData = await _repository.GetClinicData();
            return Ok(clinicData);
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] TransferController GetClinicData action {e}");
            return StatusCode(500);
        }
    }

    [HttpPost]
    public ActionResult<ClinicData> CreateClinicData(ClinicData clinicData)
    {
        try
        {
            _repository.UpdateClinicData(clinicData);
            _repository.SaveChanges();
            return CreatedAtAction(nameof(GetClinicData), null, null);
        }
        catch (Exception e)
        {
            Console.WriteLine("-->[ERROR] TransferController CreateClinicData action");
            return StatusCode(500);
        }
    }

    [HttpGet("sendData")]
    public async Task<ActionResult> SendData()
    {
        try
        {
            if (await _repository.GetClinicData() == null)
                return BadRequest("Clinic is empty");
            await gather.GetAndPutData();
            await send.SendData();
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] TransferController sendData {e}");
            return StatusCode(500);
        }
    }
}