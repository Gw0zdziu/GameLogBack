using System.Security.Claims;
using GameLogBack.Dtos.PaginatedQuery;
using GameLogBack.Dtos.PaginatedResults;
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

    public Task<PaginatedResults<T>> GetPaginatedData<T>(IQueryable<T> data, PaginatedQuery paginatedQuery);
}