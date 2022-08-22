using Authentication.Contracts;

namespace Authentication.Services.Abstraction;

public interface IApplicationUserService
{
    Task<UserDto> GetByEmailAsync(string email);
    Task RegisterAsync(RegisterUserDto registerApplicationUserDto);
    Task<TokenDto> LoginAsync(LoginUserDto loginUserDto);
}
