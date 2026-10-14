namespace GameLogBack.Settings;

public class GameBrainApiSettings
{
    public const string SectionName = "GameBrainApi";
    public string ApiUrl { get; set; }
    public string ApiKey  { get; set; }
    public string GenerateFilterOptions { get; set; }
}
