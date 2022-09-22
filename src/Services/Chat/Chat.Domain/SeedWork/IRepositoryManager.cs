using Chat.Domain.Aggregates.ServerAggregate;

namespace Chat.Domain.SeedWork;

public interface IRepositoryManager
{
    IServerRepository ServerRepository { get; }
}
