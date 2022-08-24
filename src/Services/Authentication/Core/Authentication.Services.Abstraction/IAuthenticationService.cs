using Authentication.Contracts;

namespace Authentication.Services.Abstraction;

public interface IAuthenticationService
{
    Task RegisterUserAsync(UserDto dto);
    Task<TokenDto> LoginUserAsync(UserDto dto);
    Task<TokenDto> RefreshTokenAsync(TokenDto dto);
}
