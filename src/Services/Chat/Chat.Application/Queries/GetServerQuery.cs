using Chat.Application.DTOs;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Queries;

[DataContract]
public class GetServerQuery : IRequest<ServerDto?>
{
    public GetServerQuery(Guid serverId)
    {
        ServerId = serverId;
    }

    [DataMember]
    public Guid ServerId { get; private set; }
}
