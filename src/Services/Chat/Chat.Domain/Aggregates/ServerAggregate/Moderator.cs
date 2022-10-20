namespace Chat.Domain.Aggregates.ServerAggregate;

public class Moderator
{
    public Moderator(Guid userId, DateTime dateStarted)
    {
        UserId = userId;
        DateStarted = dateStarted;
    }

    public Guid UserId { get; private set; }
    public DateTime DateStarted { get; private set; }
}
