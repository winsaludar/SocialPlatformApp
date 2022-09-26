using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class DeleteServerCommand : IRequest<bool>
{
    public DeleteServerCommand(Guid targetServerId, string deleterEmail)
    {
        TargetServerId = targetServerId;
        DeleterEmail = deleterEmail;
    }

    [DataMember]
    public Guid TargetServerId { get; private set; }

    [DataMember]
    public string DeleterEmail { get; private set; }
}
