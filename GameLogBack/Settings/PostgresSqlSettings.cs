using Microsoft.Build.Framework;

namespace GameLogBack.Settings;

public class PostgresSqlSettings
{
    public const string SectionName = "Postgres";
    
    [Required]
    public string ConnectionString { get; set; }
}