using Authentication.Contracts;
using Authentication.Domain.Entities;
using Authentication.Domain.Exceptions;
using Authentication.Domain.Repositories;
using Authentication.Services.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace Authentication.Services;

public class TokenService : ITokenService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IConfiguration _configuration;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public TokenService(IRepositoryManager repositoryManager, IConfiguration configuration, TokenValidationParameters tokenValidationParameters)
    {
        _repositoryManager = repositoryManager;
        _configuration = configuration;
        _tokenValidationParameters = tokenValidationParameters;
    }

    public async Task<TokenDto> GenerateJwtAsync(ApplicationUser user, RefreshToken? rToken = null)
    {
        if (string.IsNullOrEmpty(user.Email) || !IsEmailValid(user.Email))
            throw new InvalidEmailException(user.Email);

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

        // Refresh existing token when requested
        if (rToken != null)
        {
            return new TokenDto
            {
                Token = jwtToken,
                RefreshToken = rToken.Token,
                ExpiresAt = token.ValidTo
            };
        }

        // Create refresh token when login
        _ = int.TryParse(_configuration["JWT:RefreshTokenExpirationInMonths"], out int refreshTokenExpiration);
        RefreshToken refreshToken = new()
        {
            JwtId = token.Id,
            IsRevoked = false,
            UserId = user.Id.ToString(),
            DateAdded = DateTime.UtcNow,
            DateExpire = DateTime.UtcNow.AddMonths(refreshTokenExpiration),
            Token = $"{Guid.NewGuid()}-{Guid.NewGuid()}"
        };
        await _repositoryManager.RefreshTokenRepository.CreateAsync(refreshToken);

        return new TokenDto
        {
            Token = jwtToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = token.ValidTo
        };
    }

    public async Task<TokenDto> RefreshJwtAsync(TokenDto oldToken)
    {
        JwtSecurityTokenHandler jwtTokenHandler = new();

        var refreshTokenDb = await _repositoryManager.RefreshTokenRepository.GetByOldRefreshTokenAsync(oldToken.RefreshToken);
        if (refreshTokenDb is null)
            throw new InvalidRefreshTokenException();

        var userDb = await _repositoryManager.ApplicationUserRepository.GetByIdAsync(refreshTokenDb.UserId);
        if (userDb is null)
            throw new InvalidRefreshTokenException();

        // Handle expired token
        try
        {
            var tokenCheckResult = jwtTokenHandler.ValidateToken(oldToken.Token, _tokenValidationParameters, out var validatedToken);
            return await GenerateJwtAsync(userDb, refreshTokenDb);
        }
        catch (SecurityTokenExpiredException)
        {
            if (refreshTokenDb?.DateExpire >= DateTime.UtcNow)
                return await GenerateJwtAsync(userDb, refreshTokenDb);
            else
                return await GenerateJwtAsync(userDb);
        }
        catch (Exception)
        {
            throw new InvalidRefreshTokenException();
        }
    }

    private static bool IsEmailValid(string email)
    {
        return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
    }
}
