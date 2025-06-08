using AutoMapper;
using UserManagementService.Dtos;
using UserManagementService.Models;

namespace UserManagementService.Profiles;

public class UserManagementProfile : Profile
{
    public UserManagementProfile()
    {
        CreateMap<User, UserReadDto>();
        CreateMap<UserPublishedDto, User>();
        CreateMap<UserProfile, UserProfileReadDto>();
        CreateMap<UserProfileReadDto, UserProfile>();
        CreateMap<UserProfile, UserProfilePublishDto>();
        CreateMap<UserReceiveDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UserProfileCreateDto, UserProfile>()
            .ForMember(dest => dest.EmploymentDate,
                opt => opt.Ignore());
        CreateMap<GrpcUserResponse, UserPublishedDto>()
            .ForMember(dest => dest.Role,
                opt =>
                    opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.UserName,
                opt =>
                    opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.ExternalId,
                opt =>
                    opt.MapFrom(src => src.UserId));
    }
}