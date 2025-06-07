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
}

public enum Roles
{
    [Display(Name = "Администратор системы")] Admin,

    [Display(Name = "Врач-терапевт")] Doctor,

    [Display(Name = "Регистратор")] Registrar
}