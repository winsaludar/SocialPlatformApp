using Authentication.Contracts;
using Authentication.Domain.Entities;
using Authentication.Domain.Exceptions;
using Authentication.Domain.Repositories;
using Authentication.Services.Abstraction;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace Authentication.Services;

public class ApplicationUserService : IApplicationUserService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IConfiguration _configuration;

    public ApplicationUserService(IRepositoryManager repositoryManager, IConfiguration configuration)
    {
        _repositoryManager = repositoryManager;
        _configuration = configuration;
    }

    public async Task<ApplicationUserDto> GetByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email) || !IsEmailValid(email))
            throw new InvalidEmailException(email);

        var user = await _repositoryManager.ApplicationUserRepository.GetByEmailAsync(email);
        if (user is null)
            throw new UserNotFoundException(email);

        var userDto = user.Adapt<ApplicationUserDto>();
        return userDto;
    }

    public async Task RegisterAsync(RegisterApplicationUserDto registerApplicationUserDto)
    {
        if (string.IsNullOrEmpty(registerApplicationUserDto.Email) || !IsEmailValid(registerApplicationUserDto.Email))
            throw new InvalidEmailException(registerApplicationUserDto.Email);

        var existingUser = await _repositoryManager.ApplicationUserRepository.GetByEmailAsync(registerApplicationUserDto.Email);
        if (existingUser is not null)
            throw new UserAlreadyExistException(registerApplicationUserDto.Email);

        bool isPasswordValid = await _repositoryManager.ApplicationUserRepository.ValidateRegistrationPassword(registerApplicationUserDto.Password);
        if (!isPasswordValid)
            throw new InvalidPasswordException();

        ApplicationUser newUser = new()
        {
            FirstName = registerApplicationUserDto.FirstName,
            LastName = registerApplicationUserDto.LastName,
            Email = registerApplicationUserDto.Email
        };

        await _repositoryManager.ApplicationUserRepository.RegisterAsync(newUser, registerApplicationUserDto.Password);
    }

    public async Task<TokenDto> LoginAsync(LoginUserDto loginUserDto)
    {
        if (string.IsNullOrEmpty(loginUserDto.Email) || !IsEmailValid(loginUserDto.Email))
            throw new InvalidEmailException(loginUserDto.Email);

        var existingUser = await _repositoryManager.ApplicationUserRepository.GetByEmailAsync(loginUserDto.Email);
        if (existingUser is null)
            throw new UnauthorizedAccessException("Invalid email or password");

        bool isPasswordCorrect = await _repositoryManager.ApplicationUserRepository.ValidateLoginPassword(loginUserDto.Email, loginUserDto.Password);
        if (!isPasswordCorrect)
            throw new UnauthorizedAccessException("Invalid email or password");

        return GenerateJwtTokenAsync(existingUser);
    }

    private static bool IsEmailValid(string email)
    {
        return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
    }

    private TokenDto GenerateJwtTokenAsync(ApplicationUser user)
    {
        // Configure claims that will be added in the token
        var authClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // TODO: Add user role claims

        // Configure access token properties
        _ = int.TryParse(_configuration["JWT:ExpirationInMinutes"], out int expiration);
        var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));
        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            expires: DateTime.UtcNow.AddMinutes(expiration),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        // Generate access token
        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        // TODO: Create refresh token

        return new TokenDto
        {
            Token = jwtToken,
            RefreshToken = "",
            ExpiresAt = token.ValidTo
        };
    }
}
