namespace Chat.Application.DTOs;

public record ChannelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}
