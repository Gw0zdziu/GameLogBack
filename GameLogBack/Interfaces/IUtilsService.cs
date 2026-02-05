using System.Security.Claims;
using GameLogBack.Entities;

namespace GameLogBack.Interfaces;

public interface IUtilsService
{
    string GetRefreshToken();

    string GetToken(UserLogins userLogins, int expireIn);
    
    string GetAccessToken(UserLogins userLogins, string refreshToken);

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    
    public string GenerateCodeToConfirmEmail();
    
    public string GenerateCodeToRecoverPassword();
    
    public string GenerateLinkToRecoveryPassword(string recoverCode, string user);
}