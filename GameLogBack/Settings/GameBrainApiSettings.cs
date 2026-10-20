using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Settings;

public class GameBrainApiSettings
{
    public const string SectionName = "GameBrainApi";
    
    [Required]
    public string ApiUrl { get; set; }
    
    [Required]
    public string ApiKey  { get; set; }
    
    [Required]
    public string GenerateFilterOptions { get; set; }
}
