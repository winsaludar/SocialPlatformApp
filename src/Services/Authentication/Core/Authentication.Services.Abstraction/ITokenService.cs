using Authentication.Contracts;

namespace Authentication.Services.Abstraction;

public interface ITokenService
{
    Task<TokenDto> GenerateJwtAsync(ApplicationUserDto applicationUser, RefreshTokenDto? rToken = null);
    Task<TokenDto> RefreshJwtAsync(TokenDto oldToken);
}
