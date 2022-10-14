using Chat.Domain.Aggregates.UserAggregate;
using MediatR;
using System.Runtime.Serialization;

namespace Chat.Application.Queries;

[DataContract]
public class GetUserByEmailQuery : IRequest<User?>
{
    public GetUserByEmailQuery(string email)
    {
        Email = email;
    }

    [DataMember]
    public string Email { get; private set; }
}
