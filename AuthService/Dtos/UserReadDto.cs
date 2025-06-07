using AuthService.Models;

namespace AuthService.Dtos;

public class UserReadDto
{
    public string UserName { get; set; }
    public string Role { get; set; }
}