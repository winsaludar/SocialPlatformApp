using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class AddMessageCommand : IRequest<Guid>
{
    public AddMessageCommand(Guid serverId, Guid channelId, Guid senderId, string username, string content)
    {
        ServerId = serverId;
        ChannelId = channelId;
        SenderId = senderId;
        Username = username;
        Content = content;
    }

    [DataMember]
    public Guid ServerId { get; private set; }

    [DataMember]
    public Guid ChannelId { get; private set; }

    [DataMember]
    public Guid SenderId { get; private set; }

    [DataMember]
    public string Username { get; private set; }

    [DataMember]
    public string Content { get; private set; }
}
