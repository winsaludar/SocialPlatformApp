using Authentication.Contracts;
using Authentication.Domain.Entities;
using Authentication.Domain.Repositories;
using Authentication.Services.Abstraction;
using Mapster;

namespace Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IRepositoryManager _repositoryManager;

    public AuthenticationService(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task RegisterUserAsync(UserDto dto)
    {
        var newUser = dto.Adapt<User>();
        await newUser.RegisterAsync(dto.Password, _repositoryManager);
    }

    public async Task<TokenDto> LoginUserAsync(UserDto dto)
    {
        var user = dto.Adapt<User>();
        Token token = await user.LoginAsync(dto.Password, _repositoryManager);
        var tokenDto = token.Adapt<TokenDto>();

        return tokenDto;
    }

    public async Task<TokenDto> RefreshTokenAsync(TokenDto dto)
    {
        var token = dto.Adapt<Token>();
        await token.RefreshAsync(_repositoryManager);
        var newTokenDto = token.Adapt<TokenDto>();

        return newTokenDto;
    }


}
