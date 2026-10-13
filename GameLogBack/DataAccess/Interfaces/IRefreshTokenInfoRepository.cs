using GameLogBack.Entities;

namespace GameLogBack.DataAccess.Interfaces;

public interface IRefreshTokenInfoRepository
{
    public Task<RefreshTokenInfo> GetByUserId(string userId);
    public Task<RefreshTokenInfo> GetByRefreshToken(string token);
    public Task Create(RefreshTokenInfo refreshTokenInfo);
    public Task Update(RefreshTokenInfo refreshTokenInfo);
    public Task Delete(RefreshTokenInfo refreshTokenInfo);
}
