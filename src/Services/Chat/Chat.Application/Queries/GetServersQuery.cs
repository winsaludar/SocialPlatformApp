using Chat.Application.DTOs;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Queries;

[DataContract]
public record GetServersQuery : IRequest<IEnumerable<ServerDto>>
{
    public GetServersQuery(int page, int size)
    {
        Page = page;
        Size = size;
    }

    [DataMember]
    public int Page { get; private set; }

    [DataMember]
    public int Size { get; private set; }
}
