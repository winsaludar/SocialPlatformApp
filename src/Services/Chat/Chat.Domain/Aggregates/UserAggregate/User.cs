using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.UserAggregate;

public class User : Entity, IAggregateRoot
{
    public User(Guid authId, string username, string email)
    {
        AuthId = authId;
        Username = username;
        Email = email;
    }

    public Guid AuthId { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
}
