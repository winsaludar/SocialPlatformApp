using Authentication.Core.Contracts;
using Authentication.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Infrastructure.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly IConfiguration _configuration;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public TokenRepository(TokenValidationParameters tokenValidationParameters, IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository)
    {
        _tokenValidationParameters = tokenValidationParameters;
        _configuration = configuration;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Token> GenerateJwtAsync(User user, RefreshToken? rToken = null)
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

        // Refresh existing token when requested
        if (rToken != null)
        {
            return new Token
            {
                Value = jwtToken,
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
        await _refreshTokenRepository.CreateAsync(refreshToken);

        return new Token
        {
            Value = jwtToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = token.ValidTo
        };
    }

    public async Task<Token> RefreshJwtAsync(Token oldToken, User user, RefreshToken refreshToken)
    {
        JwtSecurityTokenHandler jwtTokenHandler = new();

        // Handle expired token
        try
        {
            var tokenCheckResult = jwtTokenHandler.ValidateToken(oldToken.Value, _tokenValidationParameters, out var validatedToken);
            return await GenerateJwtAsync(user, refreshToken);
        }
        catch (SecurityTokenExpiredException)
        {
            if (refreshToken?.DateExpire >= DateTime.UtcNow)
                return await GenerateJwtAsync(user, refreshToken);
            else
                return await GenerateJwtAsync(user);
        }
    }
}

