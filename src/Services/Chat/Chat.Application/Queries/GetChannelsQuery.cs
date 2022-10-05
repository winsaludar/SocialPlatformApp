using Chat.Application.DTOs;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Queries;

[DataContract]
public record GetChannelsQuery : IRequest<IEnumerable<ChannelDto>>
{
    public GetChannelsQuery(Guid targetServerId) => TargetServerId = targetServerId;

    public Guid TargetServerId { get; private set; }
}
