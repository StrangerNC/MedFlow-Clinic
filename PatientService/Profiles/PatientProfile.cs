using AutoMapper;
using PatientService.Dtos;
using PatientService.Models;

namespace PatientService.Profiles;

public class PatientProfile : Profile
{
    public PatientProfile()
    {
        CreateMap<Patient, PatientReadDto>();
        CreateMap<PatientCreateDto, Patient>();
        CreateMap<Patient, PatientPublishDto>();
    }
}