using Authentication.Core.DTOs;

namespace Authentication.Core.Contracts;

public interface IAuthenticationService
{
    Task<Guid> RegisterUserAsync(UserDto dto);
    Task<TokenDto> LoginUserAsync(UserDto dto);
    Task<TokenDto> RefreshTokenAsync(TokenDto dto);
}

