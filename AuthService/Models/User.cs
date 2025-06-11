using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class User
{
    [Key] [Required] public int Id { get; set; }
    [Required] public string UserName { get; set; }
    [Required] public string PasswordHash { get; set; }
    [Required] public string PasswordSalt { get; set; }
    [Required] public Roles Role { get; set; }
    [Required] public DateTime CreatedAt { get; set; }
    [Required] public DateTime LastLogin { get; set; }
    [Required] public bool IsTransferred { get; set; }
    public bool? IsActive { get; set; }
    [Required] public bool IsAdmin { get; set; }
}

public enum Roles
{
    Admin,
    Doctor,
    Registrar
}