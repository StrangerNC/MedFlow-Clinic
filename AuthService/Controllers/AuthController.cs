using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.AsyncDataService;
using AuthService.Data;
using AuthService.Dtos;
using AuthService.Models;
using AuthService.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
    [AllowAnonymous]
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
                var token = GenerateJwtToken(user);
                return Ok(new { Token = token, User = _mapper.Map<UserReadDto>(user) });
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
                _repository.CreateUser(user);
                _repository.SaveChanges();
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

    [HttpPost]
    [Route("change-password")]
    public async Task<ActionResult<UserReadDto>> ChangePassword(SignInRequestDto changePasswordRequest)
    {
        try
        {
            var user = await _repository.FindUserByUserName(changePasswordRequest.UserName);
            if (user == null)
                return Unauthorized();
            Tuple<string, string> newPassword = PasswordHasher.Hash(changePasswordRequest.Password);
            user.PasswordHash = newPassword.Item1;
            user.PasswordSalt = newPassword.Item2;
            _repository.UpdatePassword(user);
            _repository.SaveChanges();
            return Ok(_mapper.Map<UserReadDto>(user));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] AuthController change password action exception exception {e}");
            return StatusCode(500);
        }
    }

    [HttpGet("whoami")]
    public IActionResult WhoAmI()
    {
        if (!HttpContext.User.Identity!.IsAuthenticated)
            return Unauthorized("Вы не аутентифицированы");

        return Ok(new
        {
            IsAuthenticated = User.Identity.IsAuthenticated,
            Claims = User.Claims.Select(c => new { c.Type, c.Value }),
            Roles = User.Claims
                .Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                .Select(c => c.Value)
        });
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes("you-will-be-in-my-heart-forever-my-love-maftuna"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("name", user.UserName),
            new Claim("role", user.Role.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: "MedFlow.Auth",
            audience: "MedFlow.ApiGateway",
            claims: claims,
            expires: DateTime.Now.AddDays(30),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenString;
    }
}