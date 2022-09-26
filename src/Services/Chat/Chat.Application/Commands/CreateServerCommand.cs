using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public record CreateServerCommand : IRequest<Guid>
{
    public CreateServerCommand(string name, string shortDescription, string longDescription, string creatorEmail, string? thumbnail = "")
    {
        Name = name;
        ShortDescription = shortDescription;
        LongDescription = longDescription;
        CreatorEmail = creatorEmail;
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
    public string? Thumbnail { get; private set; }
}

