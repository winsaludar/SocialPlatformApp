using Chat.Domain.Aggregates.ServerAggregate;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class DeleteChannelCommand : IRequest<bool>
{
    public DeleteChannelCommand(Server targetServer, Guid targetChannelId, Guid deletedById)
    {
        TargetServer = targetServer;
        TargetChannelId = targetChannelId;
        DeletedById = deletedById;
    }

    [DataMember]
    public Server TargetServer { get; private set; }

    [DataMember]
    public Guid TargetChannelId { get; private set; }

    [DataMember]
    public Guid DeletedById { get; private set; }
}
