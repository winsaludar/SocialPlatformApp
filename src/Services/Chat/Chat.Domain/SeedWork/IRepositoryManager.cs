using Chat.Domain.Aggregates.MessageAggregate;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Aggregates.UserAggregate;

namespace Chat.Domain.SeedWork;

public interface IRepositoryManager
{
    IServerRepository ServerRepository { get; }
    IUserRepository UserRepository { get; }
    IMessageRepository MessageRepository { get; }
}
