namespace Chat.API.Models;

public record ChangeUsernameModel
{
    public Guid ServerId { get; set; }
    public string NewUsername { get; set; } = default!;
}
