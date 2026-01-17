using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Entities;

public class UserLogins
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public virtual Users User { get; set; }
}