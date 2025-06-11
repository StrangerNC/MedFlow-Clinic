using PatientService.Models;

namespace PatientService.Dtos;

public class PatientReadDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Passport { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public DateTime CreatedAt { get; set; }
}