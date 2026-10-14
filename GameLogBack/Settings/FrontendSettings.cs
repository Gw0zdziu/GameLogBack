
using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Settings;

public class FrontendSettings
{
    public const string SectionName = "Frontend";
        
    [Required] 
    public string RecoveryPasswordEndpoint { get; set; } = string.Empty;

    [Required] 
    public string FrontendUrl { get; set; } = string.Empty;
}