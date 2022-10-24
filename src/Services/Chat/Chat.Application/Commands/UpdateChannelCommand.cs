using Chat.Domain.Aggregates.ServerAggregate;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class UpdateChannelCommand : IRequest<bool>
{
    public UpdateChannelCommand(Server targetServer, Guid targetChannelId, string name, bool isPublic, Guid updatedById)
    {
        TargetServer = targetServer;
        TargetChannelId = targetChannelId;
        Name = name;
        IsPublic = isPublic;
        UpdatedById = updatedById;
    }

    [DataMember]
    public Server TargetServer { get; private set; }

    [DataMember]
    public Guid TargetChannelId { get; private set; }

    [DataMember]
    public string Name { get; private set; }

    [DataMember]
    public bool IsPublic { get; private set; }

    [DataMember]
    public Guid UpdatedById { get; private set; }
}
