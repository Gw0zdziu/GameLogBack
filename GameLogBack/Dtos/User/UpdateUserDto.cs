using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Dtos.User;

public class UpdateUserDto
{
    [Required(ErrorMessage = "FirstName is required")]
    [MinLength(3, ErrorMessage = "FirstName must be at least 3 characters long")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "LastName is required")]
    [MinLength(3, ErrorMessage = "LastName must be at least 3 characters long")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "UserEmail is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string UserEmail { get; set; }
}