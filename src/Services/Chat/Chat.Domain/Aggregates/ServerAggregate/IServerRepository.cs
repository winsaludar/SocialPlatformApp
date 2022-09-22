using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.ServerAggregate;

public interface IServerRepository : IRepository<Server>
{
    Task AddAsync(Server newServer);
}
