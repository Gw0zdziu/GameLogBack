using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Entities;

public class Users
{
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserEmail { get; set; }
    public bool IsActive { get; set; }
    public UserLogins UserLogins { get; set; }
    public RefreshTokenInfo RefreshTokens { get; set; }
    public CodeConfirmUsers CodeConfirm { get; set; }
    
    public CodeRecoveryPassword CodeRecoveryPassword { get; set; }
    
}