using Chat.Domain.Aggregates.ServerAggregate;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class AddMessageCommand : IRequest<Guid>
{
    public AddMessageCommand(Server targetServer, Guid targetChannelId, string username, string content)
    {
        TargetServer = targetServer;
        TargetChannelId = targetChannelId;
        Username = username;
        Content = content;
    }

    [DataMember]
    public Server TargetServer { get; private set; }

    [DataMember]
    public Guid TargetChannelId { get; private set; }

    [DataMember]
    public string Username { get; private set; }

    [DataMember]
    public string Content { get; private set; }
}
