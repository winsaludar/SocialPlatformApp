using Authentication.Domain.Entities;

namespace Authentication.Domain.Repositories;

public interface ITokenRepository
{
    Task<Token> GenerateJwtAsync(User user, RefreshToken? rToken = null);
    Task<Token> RefreshJwtAsync(Token oldToken, User user, RefreshToken refreshToken);
}
