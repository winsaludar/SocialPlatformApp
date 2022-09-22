using Authentication.Core.Contracts;
using Authentication.Core.DTOs;
using Authentication.Core.Models;
using Mapster;

namespace Authentication.Core.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IRepositoryManager _repositoryManager;

    public AuthenticationService(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<Guid> RegisterUserAsync(UserDto dto)
    {
        User newUser = new(_repositoryManager)
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };
        return await newUser.RegisterAsync(dto.Password);
    }

    public async Task<TokenDto> LoginUserAsync(UserDto dto)
    {
        User user = new(_repositoryManager)
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };
        Token token = await user.LoginAsync(dto.Password);
        return token.Adapt<TokenDto>();
    }

    public async Task<TokenDto> RefreshTokenAsync(TokenDto dto)
    {
        Token token = new(_repositoryManager)
        {
            Value = dto.Value,
            RefreshToken = dto.RefreshToken,
            ExpiresAt = dto.ExpiresAt
        };
        await token.RefreshAsync();
        return token.Adapt<TokenDto>();
    }


}

