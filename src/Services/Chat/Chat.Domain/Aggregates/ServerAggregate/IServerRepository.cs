using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.ServerAggregate;

public interface IServerRepository : IRepository<Server>
{
    Task<IEnumerable<Server>> GetAllAsync(int? skip = null, int? limit = null, string? nameFilter = null);
    Task<Server?> GetByNameAsync(string name);
    Task<Server?> GetByIdAsync(Guid id);
    Task<Guid> AddAsync(Server newServer);
    Task UpdateAsync(Server server);
}
