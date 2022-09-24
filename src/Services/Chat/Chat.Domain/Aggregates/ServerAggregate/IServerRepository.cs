using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.ServerAggregate;

public interface IServerRepository : IRepository<Server>
{
    Task<IEnumerable<Server>> GetAllAsync();
    Task<Server?> GetByNameAsync(string name);
    Task<Guid> AddAsync(Server newServer);
}
