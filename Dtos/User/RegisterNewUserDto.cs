using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Dtos.User;

public class RegisterNewUserDto
{
    [Required(ErrorMessage = "Username is required")]
    [MinLength(4, ErrorMessage = "Username must be at least 3 characters long")]
    public string Username { get; set; }

    [Required(ErrorMessage = "FirstName is required")]
    [MinLength(3, ErrorMessage = "FirstName must be at least 3 characters long")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "LastName is required")]
    [MinLength(3, ErrorMessage = "LastName must be at least 3 characters long")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "UserEmail is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string UserEmail { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public string Password { get; set; }

    [Required(ErrorMessage = "ConfirmPassword is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }
    
    [Required(ErrorMessage = "ConfirmPassword is required")]
    public string InvitationCode { get; set; }
}