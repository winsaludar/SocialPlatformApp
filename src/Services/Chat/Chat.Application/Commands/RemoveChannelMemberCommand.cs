using Chat.Domain.Aggregates.ServerAggregate;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class RemoveChannelMemberCommand : IRequest<bool>
{
    public RemoveChannelMemberCommand(Server targetServer, Guid channelId, Guid userId, Guid removedById)
    {
        TargetServer = targetServer;
        ChannelId = channelId;
        UserId = userId;
        RemovedById = removedById;
    }

    [DataMember]
    public Server TargetServer { get; private set; }

    [DataMember]
    public Guid ChannelId { get; private set; }

    [DataMember]
    public Guid UserId { get; private set; }

    [DataMember]
    public Guid RemovedById { get; private set; }
}
