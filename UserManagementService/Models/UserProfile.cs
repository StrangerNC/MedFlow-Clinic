using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Models;

public class UserProfile
{
    [Key] [Required] public int Id { get; set; }
    [Required] public string Position { get; set; }
    [Required] public string Department { get; set; }
    [Required] public string FullName { get; set; }
    [Required] public int UserId { get; set; }
    [Required] public DateTime EmploymentDate { get; set; }
    public bool? IsTransferred { get; set; }
    public User User { get; set; }
}