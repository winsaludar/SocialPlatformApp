namespace Authentication.Contracts;

public record TokenDto
{
    public string Token { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; } = default!;
}
