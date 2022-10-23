namespace Space.Domain.Entities;

public class SoulTopicVote
{
    public Guid SoulId { get; set; }
    public Guid TopicId { get; set; }
    public int Upvote { get; set; }
    public int Downvote { get; set; }
}
