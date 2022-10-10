using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class DeleteChannelCommand : IRequest<bool>
{
    public DeleteChannelCommand(Guid targetServerId, Guid targetChannelId)
    {
        TargetServerId = targetServerId;
        TargetChannelId = targetChannelId;
    }

    [DataMember]
    public Guid TargetServerId { get; private set; }

    [DataMember]
    public Guid TargetChannelId { get; private set; }
}
