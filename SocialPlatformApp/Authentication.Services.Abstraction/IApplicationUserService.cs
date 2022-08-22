using Authentication.Contracts;

namespace Authentication.Services.Abstraction;

public interface IApplicationUserService
{
    Task<ApplicationUserDto> GetByEmailAsync(string email);
    Task RegisterAsync(RegisterApplicationUserDto registerApplicationUserDto);
    Task<TokenDto> LoginAsync(LoginUserDto loginUserDto);
}
