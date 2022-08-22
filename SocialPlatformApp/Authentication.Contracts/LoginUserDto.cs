namespace Authentication.Contracts;

public record LoginUserDto
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
