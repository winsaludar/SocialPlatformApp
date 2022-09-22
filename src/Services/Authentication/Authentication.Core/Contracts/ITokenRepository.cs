using Authentication.Core.Models;

namespace Authentication.Core.Contracts;

public interface ITokenRepository
{
    Task<Token> GenerateJwtAsync(User user, RefreshToken? rToken = null);
    Task<Token> RefreshJwtAsync(Token oldToken, User user, RefreshToken refreshToken);
}
