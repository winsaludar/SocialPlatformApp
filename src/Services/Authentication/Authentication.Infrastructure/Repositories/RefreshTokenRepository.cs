using Authentication.Core.Contracts;
using Authentication.Infrastructure.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using CoreModels = Authentication.Core.Models;

namespace Authentication.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AuthenticationDbContext _dbContext;

    public RefreshTokenRepository(AuthenticationDbContext dbContext) => _dbContext = dbContext;

    public async Task<CoreModels.RefreshToken?> GetByOldRefreshTokenAsync(string token)
    {
        var refreshTokenDb = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
        if (refreshTokenDb is null)
            return null;

        var refreshToken = refreshTokenDb.Adapt<CoreModels.RefreshToken>();
        return refreshToken;
    }

    public async Task CreateAsync(CoreModels.RefreshToken token)
    {
        var newToken = token.Adapt<RefreshToken>();
        await _dbContext.RefreshTokens.AddAsync(newToken);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveByUserIdAsync(string userId)
    {
        var refreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == userId);
        if (refreshToken is null)
            return;

        _dbContext.RefreshTokens.Remove(refreshToken);
        await _dbContext.SaveChangesAsync();
    }
}