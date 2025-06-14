using System.ComponentModel.DataAnnotations;

namespace MedicalRecordService.Models;

public class MedicalRecord
{
    [Key] [Required] public int Id { get; set; }
    [Required] public int PatientId { get; set; }
    [Required] public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    [Required] public bool IsTransferred { get; set; }
    public Patient Patient { get; set; }
}