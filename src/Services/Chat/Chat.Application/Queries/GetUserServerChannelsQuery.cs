using Chat.Application.DTOs;
using Chat.Domain.Aggregates.ServerAggregate;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Queries;

[DataContract]
public class GetUserServerChannelsQuery : IRequest<IEnumerable<ChannelDto>>
{
    public GetUserServerChannelsQuery(Guid userId, Server targetServer)
    {
        UserId = userId;
        TargetServer = targetServer;
    }

    [DataMember]
    public Guid UserId { get; private set; }

    [DataMember]
    public Server TargetServer { get; private set; }
}
