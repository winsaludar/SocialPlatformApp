using Authentication.Core.Models;

namespace Authentication.Core.Contracts;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByOldRefreshTokenAsync(string token);
    Task CreateAsync(RefreshToken token);
    Task RemoveByUserIdAsync(string userId);
}
