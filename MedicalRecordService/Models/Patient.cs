using System.ComponentModel.DataAnnotations;

namespace MedicalRecordService.Models;

public class Patient
{
    [Key] [Required] public int Id { get; set; }
    [Required] public int ExternalId { get; set; }
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    public string? MiddleName { get; set; }
    [Required] public string PhoneNumber { get; set; }
}