using AutoMapper;
using TransferService.Models;

namespace TransferService.Profiles;

public class TransferProfile : Profile
{
    public TransferProfile()
    {
        CreateMap<GrpcAppointmentResponse, Appointment>()
            .ForMember(x => x.ExternalId, opt =>
                opt.MapFrom(x => x.AppointmentId));
        CreateMap<GrpcMedicalRecordResponse, MedicalRecord>()
            .ForMember(x => x.ExternalId, opt =>
                opt.MapFrom(x => x.MedicalRecordId));
        ;
        CreateMap<GrpcPatientResponse, Patient>()
            .ForMember(x => x.ExternalId, opt =>
                opt.MapFrom(x => x.PatientId));
        ;
        CreateMap<GrpcUserProfileResponse, UserProfile>()
            .ForMember(x => x.ExternalId, opt =>
                opt.MapFrom(x => x.UserProfileId));
        CreateMap<GrpcVisitResponse, Visit>()
            .ForMember(x => x.ExternalId, opt =>
                opt.MapFrom(x => x.VisitId))
            .ForMember(dest => dest.VisitDate, opt =>
                opt.MapFrom(src => src.VisitDate.ToDateTime()));
        CreateMap<Appointment, TransferAppointmentRequest>();
        CreateMap<MedicalRecord, TransferMedicalRecordRequest>();
        CreateMap<Patient, TransferPatientRequest>();
        CreateMap<Visit, TransferVisitRequest>();
        CreateMap<UserProfile, TransferUserProfileRequest>();
        CreateMap<ClinicData, TransferClinicDataRequest>();
    }
}