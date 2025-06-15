using System.ComponentModel.DataAnnotations;

namespace TransferService.Models;

public class UserProfile
{
    [Key] [Required] public int Id { get; set; }
    [Required] public int ExternalId { get; set; }
    [Required] public string Position { get; set; }
    [Required] public string Department { get; set; }
    [Required] public string FullName { get; set; }
    [Required] public bool IsSent { get; set; }
}