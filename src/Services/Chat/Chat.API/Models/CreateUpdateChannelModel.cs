namespace Chat.API.Models;

public record CreateUpdateChannelModel
{
    public string Name { get; set; } = default!;
}
