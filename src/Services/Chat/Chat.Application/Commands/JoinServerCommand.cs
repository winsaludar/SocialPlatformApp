using Chat.Domain.Aggregates.ServerAggregate;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class JoinServerCommand : IRequest<bool>
{
    public JoinServerCommand(Server targetServer, Guid userId, string username)
    {
        TargetServer = targetServer;
        UserId = userId;
        Username = username;
    }

    [DataMember]
    public Server TargetServer { get; private set; }

    [DataMember]
    public Guid UserId { get; private set; }

    [DataMember]
    public string Username { get; private set; }
}
