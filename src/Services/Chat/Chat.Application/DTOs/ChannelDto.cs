namespace Chat.Application.DTOs;

public record ChannelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool IsPublic { get; set; }
    public List<Guid> Members { get; set; } = new();
}
