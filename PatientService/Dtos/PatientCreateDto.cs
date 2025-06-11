using System.ComponentModel.DataAnnotations;
using PatientService.Models;

namespace PatientService.Dtos;

public class PatientCreateDto
{
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    public string? MiddleName { get; set; }
    [Required] public string Gender { get; set; }
    [Required] public DateTime DateOfBirth { get; set; }
    [Required] public string Passport { get; set; }
    [Required] public string PhoneNumber { get; set; }
    [Required] public string Address { get; set; }
}