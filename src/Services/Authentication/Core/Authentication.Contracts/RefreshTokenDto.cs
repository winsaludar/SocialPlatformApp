﻿namespace Authentication.Contracts;

public record RefreshTokenDto
{
    public Guid Id { get; set; }
    public string Token { get; set; } = default!;
    public string JwtId { get; set; } = default!;
    public bool IsRevoked { get; set; }
    public DateTime DateAdded { get; set; } = default!;
    public DateTime DateExpire { get; set; } = default!;
}
