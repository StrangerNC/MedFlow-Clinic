using System.ComponentModel.DataAnnotations;

namespace AppointmentService.Models;

public class Doctor
{
    [Key] [Required] public int Id { get; set; }
    [Required] public int ExternalId { get; set; }
    [Required] public string FullName { get; set; }
    [Required] public string Position { get; set; }
    [Required] public string Department { get; set; }
}