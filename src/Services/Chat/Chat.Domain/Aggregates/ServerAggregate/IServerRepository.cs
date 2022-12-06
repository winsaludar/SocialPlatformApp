using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.ServerAggregate;

public interface IServerRepository : IRepository<Server>
{
    Task<IEnumerable<Server>> GetAllAsync(int? skip = null, int? limit = null, string? nameFilter = null, string? categoryFilter = null);
    Task<Server?> GetByNameAsync(string name);
    Task<Server?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(Server newServer);
    Task UpdateAsync(Server server);
    Task DeleteAsync(Guid id);
}
