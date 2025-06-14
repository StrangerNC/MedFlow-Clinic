using System.ComponentModel.DataAnnotations;

namespace MedicalRecordService.Models;

public class Appointment
{
    [Key] [Required] public int Id { get; set; }
    [Required] public int ExternalId { get; set; }
    [Required] public int DoctorId { get; set; }
    [Required] public string Status { get; set; }
}