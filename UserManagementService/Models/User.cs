using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Models;

public class User
{
    [Key] [Required] public int Id { get; set; }
    [Required] public int ExternalId { get; set; }
    [Required] public string UserName { get; set; }
    [Required] public string Role { get; set; }
    public ICollection<UserProfile> UserProfiles { get; set; }
}