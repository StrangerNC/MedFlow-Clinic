using AuthService.AsyncDataService;
using AuthService.Data;
using AuthService.Dtos;
using AuthService.Models;
using AuthService.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IRepository repository, IMapper mapper, IMessageBusClient messageBusClient) : ControllerBase
{
    private readonly IRepository _repository = repository;
    private readonly IMapper _mapper = mapper;
    private readonly IMessageBusClient _messageBusClient = messageBusClient;

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
            Console.WriteLine($"-->[ERROR] AuthController getusers action exception {e}");
            return StatusCode(500);
        }
    }

    [HttpGet("{userName}", Name = nameof(GetUser))]
    public async Task<ActionResult<UserReadDto>> GetUser(string userName)
    {
        try
        {
            var user = await _repository.FindUserByUserName(userName);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserReadDto>(user));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] AuthController getuser action exception {e}");
            return StatusCode(500);
        }
    }

    [HttpGet]
    [Route("roles", Name = nameof(GetRoles))]
    public ActionResult<IEnumerable<Roles>> GetRoles()
    {
        try
        {
            return Ok(_repository.GetRoles());
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] AuthController roles action exception {e}");
            return StatusCode(500);
        }
    }

    [HttpPost]
    [Route("sign-in")]
    public async Task<ActionResult<UserReadDto>> SignIn(SignInRequestDto signInRequest)
    {
        try
        {
            var user = await _repository.FindUserByUserName(signInRequest.UserName);
            if (user == null)
                return NotFound();
            if (PasswordHasher.Verify(new Tuple<string, string>(user.PasswordHash, user.PasswordSalt),
                    signInRequest.Password))
            {
                return Ok(_mapper.Map<UserReadDto>(user));
            }

            return Unauthorized();
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] AuthController sign-in action exception {e}");
            return StatusCode(500);
        }
    }

    [HttpPost]
    [Route("sign-up")]
    public async Task<ActionResult<UserReadDto>> SignUp(SignUpRequestDto signUpRequest)
    {
        try
        {
            var user = await _repository.FindUserByUserName(signUpRequest.UserName);
            if (user == null)
            {
                user = _mapper.Map<User>(signUpRequest);
                Tuple<string, string> password = PasswordHasher.Hash(signUpRequest.Password);
                user.PasswordHash = password.Item1;
                user.PasswordSalt = password.Item2;
                user.CreatedAt = DateTime.UtcNow;
                _repository.CreateUser(user);
                await _repository.SaveChanges();
                var userPublish = _mapper.Map<UserPublishDto>(user);
                await _messageBusClient.PublishNewUser(userPublish);
                return CreatedAtAction(nameof(GetUser), new { userName = signUpRequest.UserName },
                    _mapper.Map<UserReadDto>(user));
            }

            return BadRequest();
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] AuthController sign-up action exception {e}");
        }

        return StatusCode(500);
    }
}