using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class CreateChannelCommand : IRequest<Guid>
{
    public CreateChannelCommand(Guid targetServerId, string name)
    {
        TargetServerId = targetServerId;
        Name = name;
    }

    [DataMember]
    public Guid TargetServerId { get; private set; }

    [DataMember]
    public string Name { get; private set; }
}
