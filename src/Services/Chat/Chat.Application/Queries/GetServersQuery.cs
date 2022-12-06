using Chat.Application.DTOs;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Queries;

[DataContract]
public record GetServersQuery : IRequest<IEnumerable<ServerDto>>
{
    public GetServersQuery(int page, int size, string? name, string? category)
    {
        Page = page;
        Size = size;
        Name = name;
        Category = category;
    }

    [DataMember]
    public int Page { get; private set; }

    [DataMember]
    public int Size { get; private set; }

    [DataMember]
    public string? Name { get; private set; }

    [DataMember]
    public string? Category { get; private set; }
}
