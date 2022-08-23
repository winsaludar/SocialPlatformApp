using Authentication.Domain.Exceptions;
using Authentication.Domain.Repositories;

namespace Authentication.Domain.Entities;

public class Token
{
    public string Value { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; } = default!;

    public async Task RefreshAsync(IRepositoryManager repositoryManager)
    {
        var refreshTokenDb = await repositoryManager.RefreshTokenRepository.GetByOldRefreshTokenAsync(RefreshToken);
        if (refreshTokenDb is null)
            throw new InvalidRefreshTokenException();

        var userDb = await repositoryManager.ApplicationUserRepository.GetByIdAsync(refreshTokenDb.UserId);
        if (userDb is null)
            throw new InvalidRefreshTokenException();

        try
        {
            Token newToken = await repositoryManager.TokenRepository.RefreshJwtAsync(this, userDb, refreshTokenDb);
            Value = newToken.Value;
            RefreshToken = newToken.RefreshToken;
            ExpiresAt = newToken.ExpiresAt;
        }
        catch (Exception)
        {
            throw new InvalidRefreshTokenException();
        }
    }
}
