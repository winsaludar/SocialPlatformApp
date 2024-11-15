﻿using Chat.Domain.Aggregates.ServerAggregate;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public class CreateChannelCommand : IRequest<Guid>
{
    public CreateChannelCommand(Server targetServer, string name, bool isPublic, Guid createdById)
    {
        TargetServer = targetServer;
        Name = name;
        IsPublic = isPublic;
        CreatedById = createdById;
    }

    [DataMember]
    public Server TargetServer { get; private set; }

    [DataMember]
    public string Name { get; private set; }

    [DataMember]
    public bool IsPublic { get; private set; }

    [DataMember]
    public Guid CreatedById { get; private set; }
}
