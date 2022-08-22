using Authentication.Domain.Entities;

namespace Authentication.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByOldRefreshTokenAsync(string token);
    Task CreateAsync(RefreshToken token);
    Task RemoveByUserIdAsync(string userId);
}
