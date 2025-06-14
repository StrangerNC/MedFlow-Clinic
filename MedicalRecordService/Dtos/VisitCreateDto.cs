using System.ComponentModel.DataAnnotations;

namespace MedicalRecordService.Dtos;

public class VisitCreateDto
{
    [Required] public int MedicalRecordId { get; set; }
    [Required] public int AppointmentId { get; set; }
    [Required] public string ChiefComplaint { get; set; }
    [Required] public string Diagnosis { get; set; }
    [Required] public string TreatmentPlan { get; set; }
    public string Notes { get; set; }
}