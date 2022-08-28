namespace Space.Contracts;

public record TopicDto
{
    public string AuthorEmail { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public Guid SpaceId { get; set; } = default!;
}
