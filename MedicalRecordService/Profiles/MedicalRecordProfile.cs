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

        CreateMap<MedicalRecord, MedicalRecordReadDto>();
        CreateMap<MedicalRecord, MedicalRecordPublishDto>();
        CreateMap<MedicalRecordCreateDto, MedicalRecord>();

        CreateMap<Visit, VisitReadDto>();
        CreateMap<Visit, VisitPublishDto>();
        CreateMap<VisitCreateDto, Visit>();
    }
}