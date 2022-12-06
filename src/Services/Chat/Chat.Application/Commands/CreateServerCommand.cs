using Chat.Domain.Aggregates.ServerAggregate;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public record CreateServerCommand : IRequest<Guid>
{
    public CreateServerCommand(string name, string shortDescription, string longDescription, string creatorEmail, Guid createdById, List<Category> categories, string? thumbnail = "")
    {
        Name = name;
        ShortDescription = shortDescription;
        LongDescription = longDescription;
        CreatorEmail = creatorEmail;
        CreatedById = createdById;
        Categories = categories;
        Thumbnail = thumbnail;
    }

    [DataMember]
    public string Name { get; private set; }

    [DataMember]
    public string ShortDescription { get; private set; }

    [DataMember]
    public string LongDescription { get; private set; }

    [DataMember]
    public string CreatorEmail { get; private set; }

    [DataMember]
    public Guid CreatedById { get; private set; }

    [DataMember]
    public List<Category> Categories { get; private set; }

    [DataMember]
    public string? Thumbnail { get; private set; }
}

