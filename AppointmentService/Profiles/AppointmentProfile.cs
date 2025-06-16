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
        CreateMap<Doctor, DoctorReadDto>()
            .ForMember(x => x.Id, opt =>
                opt.MapFrom(x => x.ExternalId));
        CreateMap<DoctorPublishedDto, Doctor>();
        CreateMap<Appointment, AppointmentReadDto>()
            .ForMember(x => x.PatientId, opt =>
                opt.MapFrom(x => x.Patient.ExternalId))
            .ForMember(x => x.DoctorId, opt =>
                opt.MapFrom(x => x.Doctor.ExternalId));
        CreateMap<AppointmentCreateDto, Appointment>();
        CreateMap<Appointment, AppointmentPublishDto>()
            .ForMember(x => x.PatientId, opt =>
                opt.MapFrom(x => x.Patient.ExternalId))
            .ForMember(x => x.DoctorId, opt =>
                opt.MapFrom(x => x.Doctor.ExternalId));
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