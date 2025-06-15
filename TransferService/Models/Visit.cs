using System.ComponentModel.DataAnnotations;

namespace TransferService.Models;

public class Visit
{
    [Key] [Required] public int Id { get; set; }
    [Required] public int ExternalId { get; set; }
    [Required] public int MedicalRecordId { get; set; }
    [Required] public int AppointmentId { get; set; }
    [Required] public DateTime VisitDate { get; set; }
    [Required] public string ChiefComplaint { get; set; }
    [Required] public string Diagnosis { get; set; }
    [Required] public string TreatmentPlan { get; set; }
    public string Notes { get; set; }
    [Required] public bool IsSent { get; set; }
}