using Authentication.Contracts;
using Authentication.Domain.Entities;

namespace Authentication.Services.Abstraction;

public interface ITokenService
{
    Task<TokenDto> GenerateJwtAsync(ApplicationUser user, RefreshToken? rToken = null);
    Task<TokenDto> RefreshJwtAsync(TokenDto oldToken);
}
