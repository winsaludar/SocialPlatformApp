using Chat.Domain.Aggregates.ServerAggregate;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class CreateChannelCommand : IRequest<Guid>
{
    public CreateChannelCommand(Server targetServer, string name, string createdBy)
    {
        TargetServer = targetServer;
        Name = name;
        CreatedBy = createdBy;
    }

    [DataMember]
    public Server TargetServer { get; private set; }

    [DataMember]
    public string Name { get; private set; }

    [DataMember]
    public string CreatedBy { get; private set; }
}
