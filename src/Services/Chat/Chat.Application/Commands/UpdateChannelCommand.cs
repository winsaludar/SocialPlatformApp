using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class UpdateChannelCommand : IRequest<bool>
{
    public UpdateChannelCommand(Guid targetServerId, Guid targetChannelId, string name, string updatedBy)
    {
        TargetServerId = targetServerId;
        TargetChannelId = targetChannelId;
        Name = name;
        UpdatedBy = updatedBy;
    }

    [DataMember]
    public Guid TargetServerId { get; private set; }

    [DataMember]
    public Guid TargetChannelId { get; private set; }

    [DataMember]
    public string Name { get; private set; }

    [DataMember]
    public string UpdatedBy { get; private set; }
}
