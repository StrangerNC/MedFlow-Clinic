using AutoMapper;
using MedicalRecordService.Dtos;
using MedicalRecordService.Models;
using MedicalRecordService.Utils;

namespace MedicalRecordService.Profiles;

public class MedicalRecordProfile : Profile
{
    public MedicalRecordProfile()
    {
        CreateMap<AppointmentPublishedDto, Appointment>();
        CreateMap<GrpcAppointmentResponse, AppointmentPublishedDto>();

        CreateMap<PatientPublishedDto, Patient>();
        CreateMap<GrpcPatientResponse, PatientPublishedDto>();

        CreateMap<MedicalRecord, MedicalRecordReadDto>()
            .ForMember(x => x.PatientId, opt =>
                opt.MapFrom(x => x.Patient.ExternalId));
        CreateMap<MedicalRecord, MedicalRecordPublishDto>()
            .ForMember(x => x.PatientId, opt =>
                opt.MapFrom(x => x.Patient.ExternalId));
        CreateMap<MedicalRecordCreateDto, MedicalRecord>();

        CreateMap<Visit, VisitReadDto>()
            .ForMember(x => x.AppointmentId, opt =>
                opt.MapFrom(x => x.Appointment.ExternalId));
        CreateMap<Visit, VisitPublishDto>()
            .ForMember(x => x.AppointmentId, opt =>
                opt.MapFrom(x => x.Appointment.ExternalId));
        CreateMap<VisitCreateDto, Visit>();
    }
}