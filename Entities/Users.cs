using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Entities;

public class Users
{
    [Key]
    [Required]
    [MaxLength(100)]
    public string UserId { get; set; }
    [Required]
    [MaxLength(100)]
    public string UserName { get; set; }
    [Required]
    [MaxLength(100)]
    public string UserEmail { get; set; }
    public UserLogins UserLogins { get; set; }
}