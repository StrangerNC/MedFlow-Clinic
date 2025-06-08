using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserManagementService.Data;
using UserManagementService.Dtos;
using UserManagementService.Models;

namespace UserManagementService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IRepository repository, IMapper mapper) : ControllerBase
{
    private readonly IRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserReadDto>>> GetUsers()
    {
        try
        {
            var users = await _repository.GetUsers();
            return Ok(_mapper.Map<IEnumerable<UserReadDto>>(users));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] UserController GetUsers action exception {e}");
            return StatusCode(500);
        }
    }

    [HttpGet("{id}", Name = nameof(GetUser))]
    public async Task<ActionResult<UserReadDto>> GetUser(int id)
    {
        try
        {
            var user = await _repository.GetUserById(id);
            if (user == null)
                return NotFound();
            return Ok(_mapper.Map<UserReadDto>(user));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] UserController GetUser action exception {e}");
            return StatusCode(500);
        }
    }
}