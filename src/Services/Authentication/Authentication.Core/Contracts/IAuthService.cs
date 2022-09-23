using Authentication.Core.Models;

namespace Authentication.Core.Contracts;

public interface IAuthService
{
    Task<Guid> RegisterUserAsync(User newUser, string password);
    Task<Token> LoginUserAsync(User user, string password);
    Task<Token> RefreshTokenAsync(Token oldToken);
}

