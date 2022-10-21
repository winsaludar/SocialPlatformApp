using Chat.Domain.Aggregates.ServerAggregate;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class RemoveModeratorCommand : IRequest<bool>
{
    public RemoveModeratorCommand(Server targetServer, Guid userId, Guid removeById)
    {
        TargetServer = targetServer;
        UserId = userId;
        RemoveById = removeById;
    }

    [DataMember]
    public Server TargetServer { get; private set; }
    public Guid UserId { get; private set; }
    public Guid RemoveById { get; private set; }
}
