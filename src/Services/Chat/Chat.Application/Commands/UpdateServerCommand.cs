using Chat.Domain.Aggregates.ServerAggregate;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public record UpdateServerCommand : IRequest<bool>
{
    public UpdateServerCommand(Server targetServer, string name, string shortDescription, string longDescription, string editorEmail, string? thumbnail = "")
    {
        TargetServer = targetServer;
        Name = name;
        ShortDescription = shortDescription;
        LongDescription = longDescription;
        EditorEmail = editorEmail;
        Thumbnail = thumbnail;
    }

    [DataMember]
    public Server TargetServer { get; private set; }

    [DataMember]
    public string Name { get; private set; }

    [DataMember]
    public string ShortDescription { get; private set; }

    [DataMember]
    public string LongDescription { get; private set; }

    [DataMember]
    public string EditorEmail { get; private set; }

    [DataMember]
    public string? Thumbnail { get; private set; }
}
