using System.ComponentModel.DataAnnotations;
using AuthService.Models;

namespace AuthService.Dtos;

public class SignUpRequestDto
{
    [Required] public string UserName { get; set; }
    [Required] public string Password { get; set; }
    [Required] public string Role { get; set; }
}