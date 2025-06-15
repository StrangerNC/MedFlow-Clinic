using System.ComponentModel.DataAnnotations;

namespace TransferService.Models;

public class Appointment
{
    [Key] [Required] public int Id { get; set; }
    [Required] public int ExternalId { get; set; }
    [Required] public int PatientId { get; set; }
    [Required] public int DoctorId { get; set; }
    [Required] public string Status { get; set; }
    [Required] public string Reason { get; set; }
    [Required] public bool IsSent { get; set; }
}