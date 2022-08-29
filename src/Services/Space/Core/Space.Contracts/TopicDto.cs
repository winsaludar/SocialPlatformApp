namespace Space.Contracts;

public record TopicDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string AuthorEmail { get; set; } = default!;
    public string AuthorUsername { get; set; } = default!;
    public DateTime CreatedDateUtc { get; set; }

    public Guid SpaceId { get; set; } = default!;
    public Guid SoulId { get; set; } = default!;
}
