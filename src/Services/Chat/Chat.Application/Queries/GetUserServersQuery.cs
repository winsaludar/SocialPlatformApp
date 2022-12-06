using Chat.Application.DTOs;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Queries;

[DataContract]
public class GetUserServersQuery : IRequest<IEnumerable<ServerDto>>
{
    public GetUserServersQuery(Guid userId)
    {
        UserId = userId;
    }

    [DataMember]
    public Guid UserId { get; private set; }
}
