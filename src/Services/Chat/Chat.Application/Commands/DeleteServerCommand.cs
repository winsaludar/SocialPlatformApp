using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class DeleteServerCommand : IRequest<bool>
{
    public DeleteServerCommand(Guid targetServerId, Guid deletedById)
    {
        TargetServerId = targetServerId;
        DeletedById = deletedById;
    }

    [DataMember]
    public Guid TargetServerId { get; private set; }

    [DataMember]
    public Guid DeletedById { get; private set; }
}
