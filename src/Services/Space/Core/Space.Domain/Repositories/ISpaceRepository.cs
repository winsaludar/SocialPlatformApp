using DomainEntities = Space.Domain.Entities;

namespace Space.Domain.Repositories;

public interface ISpaceRepository
{
    Task CreateAsync(DomainEntities.Space newSpace);
}
