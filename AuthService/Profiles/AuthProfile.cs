using AuthService.Dtos;
using AuthService.Models;
using AutoMapper;

namespace AuthService.Profiles;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<User, UserReadDto>();
        CreateMap<SignInRequestDto, User>();
        CreateMap<SignUpRequestDto, User>();
        CreateMap<User, UserPublishDto>();
    }
}