namespace Space.Contracts;

public record CommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public string AuthorEmail { get; set; } = default!;
    public string AuthorUsername { get; set; } = default!;
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
    public DateTime CreatedDateUtc { get; set; }

    public Guid SpaceId { get; set; } = default!;
    public Guid TopicId { get; set; } = default!;
    public Guid SoulId { get; set; } = default!;
}
