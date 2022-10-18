namespace Chat.Domain.Aggregates.ServerAggregate;

public class Member
{
    public Member(Guid userId, string username, DateTime dateJoined)
    {
        UserId = userId;
        Username = username;
        DateJoined = dateJoined;
    }

    public Guid UserId { get; private set; }
    public string Username { get; private set; }
    public DateTime DateJoined { get; private set; }
}
