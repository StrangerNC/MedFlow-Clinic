using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserManagementService.AsyncDataService;
using UserManagementService.Data;
using UserManagementService.Dtos;
using UserManagementService.Models;
using UserManagementService.SyncDataService;

namespace UserManagementService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserProfileController(
    IRepository repository,
    IMapper mapper,
    IAuthDataClient grpcClient,
    IMessageBusClient messageBus) : ControllerBase
{
    private readonly IRepository _repository = repository;
    private readonly IMapper _mapper = mapper;
    private readonly IAuthDataClient _grpcClient = grpcClient;
    private readonly IMessageBusClient _messageBus = messageBus;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserProfileReadDto>>> GetUserProfiles()
    {
        try
        {
            var userProfiles = await _repository.GetUserProfiles();
            return Ok(_mapper.Map<IEnumerable<UserProfileReadDto>>(userProfiles));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] UserProfileController GetUserProfiles action exception {e}");
            return StatusCode(500);
        }
    }

    [HttpGet("{userProfileId}", Name = nameof(GetUserProfile))]
    public async Task<ActionResult<UserProfileReadDto>> GetUserProfile(int userProfileId)
    {
        try
        {
            var userProfile = await _repository.GetUserProfileById(userProfileId);
            return Ok(_mapper.Map<UserProfileReadDto>(userProfile));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] UserProfileController GetUserProfile action exception {e}");
            return StatusCode(500);
        }
    }

    [HttpPut("{userProfileId}")]
    public async Task<ActionResult<UserProfileReadDto>> UpdateUserProfile(int userProfileId,
        UserProfileReadDto userProfileRead)
    {
        try
        {
            var userProfile = await _repository.GetUserProfileById(userProfileId);
            if (userProfile == null)
                return NotFound();
            userProfile.Department = userProfileRead.Department;
            userProfile.FullName = userProfileRead.FullName;
            userProfile.Position = userProfileRead.Position;
            _repository.UpdateUserProfile(userProfile);
            _repository.SaveChanges();
            userProfileRead = _mapper.Map<UserProfileReadDto>(userProfile);
            return CreatedAtRoute(nameof(GetUserProfile), new { userProfileId = userProfile.Id }, userProfileRead);
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] UserProfileController UpdateUserProfile action exception {e}");
            return StatusCode(500);
        }
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult<UserProfileReadDto>> CreateUserProfile(int userId,
        UserProfileCreateDto userCreateDto)
    {
        //ToDo Rewrite code to work with externalUserId, it will be easier for apigateway
        try
        {
            var userExists = await _repository.GetUserByExternalId(userId);
            if (userExists == null)
            {
                PrepDb.SeedAuthData(_grpcClient, _repository);
                var doubleCheckUserExists = await _repository.GetUserByExternalId(userId);
                if (doubleCheckUserExists == null)
                    return NotFound();
            }

            var userProfileExists = _repository.UserProfileExistByUserId(userId);
            if (userProfileExists)
                return BadRequest();
            var userProfile = _mapper.Map<UserProfile>(userCreateDto);
            userProfile.UserId = userExists.Id;
            userProfile.EmploymentDate = Convert.ToDateTime(userCreateDto.EmploymentDate).ToUniversalTime();
            _repository.CreateUserProfile(userProfile);
            _repository.SaveChanges();
            var userProfileRead = _mapper.Map<UserProfileReadDto>(userProfile);
            var userProfilePublished = _mapper.Map<UserProfilePublishDto>(userProfile);
            await _messageBus.PublishNewUserProfile(userProfilePublished);
            return CreatedAtRoute(nameof(GetUserProfile), new { userProfileId = userProfile.Id }, userProfileRead);
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] UserProfileController CreateUserProfile action exception {e}");
            return StatusCode(500);
        }
    }
}