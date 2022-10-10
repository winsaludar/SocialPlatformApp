using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class CreateChannelCommand : IRequest<Guid>
{
    public CreateChannelCommand(Guid targetServerId, string name, string createdBy)
    {
        TargetServerId = targetServerId;
        Name = name;
        CreatedBy = createdBy;
    }

    [DataMember]
    public Guid TargetServerId { get; private set; }

    [DataMember]
    public string Name { get; private set; }

    [DataMember]
    public string CreatedBy { get; private set; }
}
