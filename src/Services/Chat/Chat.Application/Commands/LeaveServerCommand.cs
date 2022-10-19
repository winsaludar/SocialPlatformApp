using Chat.Domain.Aggregates.ServerAggregate;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class LeaveServerCommand : IRequest<bool>
{
    public LeaveServerCommand(Server targetServer, Guid userId)
    {
        TargetServer = targetServer;
        UserId = userId;
    }

    [DataMember]
    public Server TargetServer { get; private set; }

    [DataMember]
    public Guid UserId { get; private set; }
}
