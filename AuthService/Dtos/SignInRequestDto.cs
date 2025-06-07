using System.ComponentModel.DataAnnotations;

namespace AuthService.Dtos;

public class SignInRequestDto
{
    [Required] public string UserName { get; set; }
    [Required] public string Password { get; set; }
}