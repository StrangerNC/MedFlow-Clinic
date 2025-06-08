using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Dtos;

public class UserProfileCreateDto
{
    [Required] public string Position { get; set; }
    [Required] public string FullName { get; set; }
    [Required] public string EmploymentDate { get; set; }
    [Required] public string Department { get; set; }
}