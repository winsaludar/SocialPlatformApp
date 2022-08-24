using DomainEntities = Space.Domain.Entities;

namespace Space.Domain.Repositories;

public interface ISpaceRepository
{
    Task<IEnumerable<DomainEntities.Space>> GetAllAsync();
    Task<DomainEntities.Space?> GetByNameAsync(string name);
    Task<DomainEntities.Space?> GetByIdAsync(Guid id);
    Task CreateAsync(DomainEntities.Space newSpace);
    Task UpdateAsync(DomainEntities.Space space);
}
