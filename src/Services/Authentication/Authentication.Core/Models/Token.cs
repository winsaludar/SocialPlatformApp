namespace Authentication.Core.Models;

public class Token
{
    public Token() { }

    public Token(string value, string refreshToken, DateTime expiresAt)
    {
        Value = value;
        RefreshToken = refreshToken;
        ExpiresAt = expiresAt;
    }

    public string Value { get; private set; } = default!;
    public string RefreshToken { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
}
