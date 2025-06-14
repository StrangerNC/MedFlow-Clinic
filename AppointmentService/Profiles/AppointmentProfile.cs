using AppointmentService.Dtos;
using AppointmentService.Models;
using AutoMapper;

namespace AppointmentService.Profiles;

public class AppointmentProfile : Profile
{
    public AppointmentProfile()
    {
        CreateMap<Patient, PatientReadDto>();
        CreateMap<PatientPublishedDto, Patient>();
        CreateMap<Doctor, DoctorReadDto>();
        CreateMap<DoctorPublishedDto, Doctor>();
        CreateMap<Appointment, AppointmentReadDto>();
        CreateMap<AppointmentCreateDto, Appointment>();
        CreateMap<Appointment, AppointmentPublishDto>();
        CreateMap<PatientReceivedDto, Patient>().ForMember(x => x.Id, opt =>
            opt.Ignore());
        CreateMap<DoctorReceivedDto, Doctor>().ForMember(x => x.Id, opt =>
            opt.Ignore());
        CreateMap<GrpcPatientResponse, PatientPublishedDto>()
            .ForMember(x => x.ExternalId, opt =>
                opt.MapFrom(x => x.PatientId));
        CreateMap<GrpcUserProfileResponse, DoctorPublishedDto>()
            .ForMember(x => x.ExternalId, opt =>
                opt.MapFrom(x => x.UserProfileId));
    }
}