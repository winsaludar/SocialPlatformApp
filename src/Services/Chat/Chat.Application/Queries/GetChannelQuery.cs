using Chat.Application.DTOs;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Queries;

[DataContract]
public class GetChannelQuery : IRequest<ChannelDto?>
{
    public GetChannelQuery(Guid serverId, Guid channelId)
    {
        ServerId = serverId;
        ChannelId = channelId;
    }

    [DataMember]
    public Guid ServerId { get; private set; }

    [DataMember]
    public Guid ChannelId { get; private set; }
}
