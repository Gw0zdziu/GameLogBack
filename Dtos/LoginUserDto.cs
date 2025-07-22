using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Dtos;

public class LoginUserDto
{
    [Required(ErrorMessage = "Username is required")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}