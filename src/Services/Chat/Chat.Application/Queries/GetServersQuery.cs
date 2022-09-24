using Chat.Application.DTOs;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Queries;

[DataContract]
public record GetServersQuery : IRequest<IEnumerable<ServerDto>>
{
    public GetServersQuery(int page, int size, string? name)
    {
        Page = page;
        Size = size;
        Name = name;
    }

    [DataMember]
    public int Page { get; private set; }

    [DataMember]
    public int Size { get; private set; }

    [DataMember]
    public string? Name { get; private set; }
}
