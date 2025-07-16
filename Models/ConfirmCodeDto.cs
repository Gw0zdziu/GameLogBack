using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Models;

public class ConfirmCodeDto
{
    [Required(ErrorMessage = "UserId is required")]
    public string UserId { get; set; }
    
    [Required(ErrorMessage = "ConfirmCode is required")] 
    [MinLength(4,ErrorMessage = "ConfirmCode must be at least 4 characters long")]
    [RegularExpression("^[0-9]+$", ErrorMessage = "ConfirmCode must be digits only")]
    public string ConfirmCode { get; set; }
}