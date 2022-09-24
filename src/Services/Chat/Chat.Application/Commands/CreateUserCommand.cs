using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Commands;

[DataContract]
public record CreateUserCommand : IRequest<Guid>
{
    public CreateUserCommand(Guid authId, string username, string email, Guid createdById)
    {
        AuthId = authId;
        Username = username;
        Email = email;
        CreatedById = createdById;
    }

    [DataMember]
    public Guid AuthId { get; private set; }

    [DataMember]
    public string Username { get; private set; } = default!;

    [DataMember]
    public string Email { get; private set; } = default!;

    [DataMember]
    public Guid CreatedById { get; set; } = default!;
}
