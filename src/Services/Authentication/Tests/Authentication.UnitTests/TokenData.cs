using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Authentication.UnitTests;

public static class TokenData
{
    public static TokenValidationParameters GetTokenValidationParameters(IConfiguration config) => new()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["JWT:Secret"])),
        ValidateIssuer = true,
        ValidIssuer = config["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = config["JWT:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
}
