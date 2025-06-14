using System.ComponentModel.DataAnnotations;

namespace MedicalRecordService.Dtos;

public class MedicalRecordCreateDto
{
    [Required] public int PatientId { get; set; }
}