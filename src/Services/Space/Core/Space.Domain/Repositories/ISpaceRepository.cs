using DomainEntities = Space.Domain.Entities;

namespace Space.Domain.Repositories;

public interface ISpaceRepository
{
    Task<IEnumerable<DomainEntities.Space>> GetAllAsync(bool includeSouls = false);
    Task<DomainEntities.Space?> GetByNameAsync(string name, bool includeSouls = false);
    Task<DomainEntities.Space?> GetByIdAsync(Guid id, bool includeSouls = false);
    Task CreateAsync(DomainEntities.Space newSpace);
    Task UpdateAsync(DomainEntities.Space space);
}
