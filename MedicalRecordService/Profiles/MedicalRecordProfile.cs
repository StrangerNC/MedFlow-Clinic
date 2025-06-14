using AutoMapper;
using MedicalRecordService.Dtos;
using MedicalRecordService.Models;

namespace MedicalRecordService.Profiles;

public class MedicalRecordProfile : Profile
{
    public MedicalRecordProfile()
    {
        CreateMap<AppointmentPublishedDto, Appointment>();
        
        CreateMap<PatientPublishedDto, Patient>();
        
        CreateMap<MedicalRecord, MedicalRecordReadDto>();
        CreateMap<MedicalRecord, MedicalRecordPublishDto>();
        CreateMap<MedicalRecordCreateDto, MedicalRecord>();
        
        CreateMap<Visit, VisitReadDto>();
        CreateMap<Visit, VisitPublishDto>();
        CreateMap<VisitCreateDto, Visit>();
    }
}