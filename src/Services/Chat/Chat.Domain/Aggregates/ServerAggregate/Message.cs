using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.ServerAggregate;

public class Message : Entity
{
    public Message(string username, string content)
    {
        Username = username;
        Content = content;
    }

    public string Username { get; private set; }
    public string Content { get; private set; }
}
