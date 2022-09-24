namespace Chat.Application.DTOs;

public record ServerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public string LongDescription { get; set; } = default!;
    public string? Thumbnail { get; set; } = default!;
}
