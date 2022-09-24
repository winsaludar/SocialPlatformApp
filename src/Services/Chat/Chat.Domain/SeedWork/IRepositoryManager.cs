using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Aggregates.UserAggregate;

namespace Chat.Domain.SeedWork;

public interface IRepositoryManager
{
    IServerRepository ServerRepository { get; }
    IUserRepository UserRepository { get; }
}
