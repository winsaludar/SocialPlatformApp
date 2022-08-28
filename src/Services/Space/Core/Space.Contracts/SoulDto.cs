namespace Space.Contracts;

public record SoulDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
}
