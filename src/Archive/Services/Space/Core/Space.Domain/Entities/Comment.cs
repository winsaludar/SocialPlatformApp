namespace Space.Domain.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = default!;
    public Guid TopicId { get; set; }
    public Guid? SoulId { get; set; }
    public Topic Topic { get; set; } = default!;
    public Soul? Soul { get; set; }
}
