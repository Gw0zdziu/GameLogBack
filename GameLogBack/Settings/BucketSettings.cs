
using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Settings;

public class BucketSettings
{
    public const string SectionName = "Bucket";

    [Required]
    public string BucketName { get; set; }
    
    [Required]
    public string AccessKey { get; set; }
    
    [Required]
    public string SecretKey { get; set; }
    
    [Required]
    public string ServiceUrl { get; set; }
    
    [Required]
    public bool ForcePathStyle { get; set; }
    
    [Required]
    public string GamesImageDirectoryName { get; set; }
}