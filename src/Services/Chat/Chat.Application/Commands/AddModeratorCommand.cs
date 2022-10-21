using Chat.Domain.Aggregates.ServerAggregate;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class AddModeratorCommand : IRequest<bool>
{
    public AddModeratorCommand(Server targetServer, Guid userId, Guid addedById)
    {
        TargetServer = targetServer;
        UserId = userId;
        AddedById = addedById;
    }

    [DataMember]
    public Server TargetServer { get; private set; }

    [DataMember]
    public Guid UserId { get; private set; }

    [DataMember]
    public Guid AddedById { get; private set; }
}
