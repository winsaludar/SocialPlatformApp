using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.MessageAggregate;

public class Message : Entity, IAggregateRoot
{
    public Message(Guid serverId, Guid channelId, Guid senderId, string username, string content)
    {
        ServerId = serverId;
        ChannelId = channelId;
        SenderId = senderId;
        Username = username;
        Content = content;
    }

    public Guid ServerId { get; private set; }
    public Guid ChannelId { get; private set; }
    public Guid SenderId { get; private set; }
    public string Username { get; private set; }
    public string Content { get; private set; }
}
