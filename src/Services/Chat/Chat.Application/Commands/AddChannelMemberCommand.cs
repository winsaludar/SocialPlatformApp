using Chat.Domain.Aggregates.ServerAggregate;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class AddChannelMemberCommand : IRequest<bool>
{
    public AddChannelMemberCommand(Server targetServer, Guid channelId, Guid userId, Guid addedById)
    {
        TargetServer = targetServer;
        ChannelId = channelId;
        UserId = userId;
        AddedById = addedById;
    }

    [DataMember]
    public Server TargetServer { get; private set; }

    [DataMember]
    public Guid ChannelId { get; private set; }

    [DataMember]
    public Guid UserId { get; private set; }

    [DataMember]
    public Guid AddedById { get; private set; }
}
