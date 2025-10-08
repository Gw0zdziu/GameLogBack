using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Entities;

public class Users
{
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserEmail { get; set; }
    public bool IsActive { get; set; }
    public virtual UserLogins UserLogins { get; set; }
    public virtual RefreshTokenInfo RefreshTokens { get; set; }
    public virtual CodeConfirmUsers CodeConfirm { get; set; }
    public virtual CodeRecoveryPassword CodeRecoveryPassword { get; set; }
    public virtual ICollection<Categories> Categories { get; set; }
    
}