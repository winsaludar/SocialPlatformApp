namespace Chat.Infrastructure.Models;

public class MessageDbModel : EntityDbModel
{
    public string Username { get; set; } = default!;
    public string Content { get; set; } = default!;
}
