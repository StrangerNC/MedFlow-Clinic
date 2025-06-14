using System.ComponentModel.DataAnnotations;

namespace AppointmentService.Dtos;

public class AppointmentCreateDto
{
    [Required] public int PatientExternalId { get; set; }
    [Required] public int DoctorExternalId { get; set; }
    [Required] public DateTime AppointmentDate { get; set; }
    [Required] public string Status { get; set; }
    [Required] public string Reason { get; set; }
}