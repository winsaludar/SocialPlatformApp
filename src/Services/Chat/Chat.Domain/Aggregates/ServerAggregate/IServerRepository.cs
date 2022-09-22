using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.ServerAggregate;

public interface IServerRepository : IRepository<Server>
{
    Task<Server?> GetByNameAsync(string name);
    Task AddAsync(Server newServer);
}
