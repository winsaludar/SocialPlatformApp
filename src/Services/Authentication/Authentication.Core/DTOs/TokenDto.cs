namespace Authentication.Core.DTOs;

public record TokenDto
{
    public string Value { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; } = default!;
}

