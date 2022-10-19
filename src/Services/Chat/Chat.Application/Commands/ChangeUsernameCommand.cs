using Chat.Domain.Aggregates.ServerAggregate;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class ChangeUsernameCommand : IRequest<bool>
{
    public ChangeUsernameCommand(Server targetServer, Guid userId, string newUsername)
    {
        TargetServer = targetServer;
        UserId = userId;
        NewUsername = newUsername;
    }

    [DataMember]
    public Server TargetServer { get; private set; }

    [DataMember]
    public Guid UserId { get; private set; }

    [DataMember]
    public string NewUsername { get; private set; }
}
