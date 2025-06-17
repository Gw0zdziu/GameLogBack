using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Entities;

public class UserLogins
{
    [Key]
    [Required]
    [MaxLength(100)]
    public string UserLoginId { get; set; }
    [Required]
    [MaxLength(100)]
    public string Password { get; set; }
    [Required]
    [MaxLength(100)]
    public string UserId { get; set; }
    public Users User { get; set; }
}