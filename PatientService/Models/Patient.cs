using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PatientService.Models;

public class Patient
{
    [Key] [Required] public int Id { get; set; }
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    public string? MiddleName { get; set; }
    [Required] public Genders Gender { get; set; }
    [Required] public DateTime DateOfBirth { get; set; }
    [Required] public string Passport { get; set; }
    [Required] public string PhoneNumber { get; set; }
    [Required] public string Address { get; set; }
    public DateTime CreatedAt { get; set; }
    [Required] public bool IsTransferred { get; set; }
}

public enum Genders
{
    Male,
    Female
}