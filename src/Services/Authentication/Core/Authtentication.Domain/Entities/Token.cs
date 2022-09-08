using Authentication.Domain.Exceptions;
using Authentication.Domain.Repositories;

namespace Authentication.Domain.Entities;

public class Token
{
    private readonly IRepositoryManager? _repositoryManager;

    public Token() { }

    public Token(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public string Value { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; } = default!;

    public async Task RefreshAsync()
    {
        if (_repositoryManager == null)
            throw new NullReferenceException("IRepositoryManager is null");

        var refreshTokenDb = await _repositoryManager.RefreshTokenRepository.GetByOldRefreshTokenAsync(RefreshToken);
        if (refreshTokenDb is null)
            throw new InvalidRefreshTokenException();

        var userDb = await _repositoryManager.UserRepository.GetByIdAsync(refreshTokenDb.UserId);
        if (userDb is null)
            throw new InvalidRefreshTokenException();

        try
        {
            Token newToken = await _repositoryManager.TokenRepository.RefreshJwtAsync(this, userDb, refreshTokenDb);
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
