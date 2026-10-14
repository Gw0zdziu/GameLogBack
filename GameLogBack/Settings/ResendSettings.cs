using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Settings;

public class ResendSettings
{
    public const string SectionName = "Resend";

    [Required] 
    public string ApiToken { get; set; }
    
    [Required]
    public string EmailAddress { get; set; }
    
    [Required]
    public bool ThrowExceptions { get; set; } 
    
}