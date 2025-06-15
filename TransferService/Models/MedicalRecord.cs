using System.ComponentModel.DataAnnotations;

namespace TransferService.Models;

public class MedicalRecord
{
    [Key] [Required] public int Id { get; set; }
    [Required] public int ExternalId { get; set; }
    [Required] public int PatientId { get; set; }
    [Required] public bool IsSent { get; set; }
}