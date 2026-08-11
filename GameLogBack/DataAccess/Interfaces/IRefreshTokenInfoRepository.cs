using GameLogBack.Entities;

namespace GameLogBack.DataAccess.Interfaces;

public interface IRefreshTokenInfoRepository
{
    public Task<RefreshTokenInfo> GetByUserId(string token);
    public Task Create(RefreshTokenInfo refreshTokenInfo);
    public Task Update(RefreshTokenInfo refreshTokenInfo);
    public Task Delete(RefreshTokenInfo refreshTokenInfo);
}
