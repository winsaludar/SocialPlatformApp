using Authentication.Contracts;

namespace Authentication.Services.Abstraction;

public interface ITokenService
{
    Task<TokenDto> GenerateJwtAsync(UserDto applicationUser, RefreshTokenDto? rToken = null);
    Task<TokenDto> RefreshJwtAsync(TokenDto oldToken);
}
