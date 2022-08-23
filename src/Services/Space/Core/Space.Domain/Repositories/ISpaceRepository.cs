using DomainEntities = Space.Domain.Entities;

namespace Space.Domain.Repositories;

public interface ISpaceRepository
{
    Task<DomainEntities.Space?> GetByNameAsync(string name);
    Task CreateAsync(DomainEntities.Space newSpace);
}
