using Space.Domain.Entities;

namespace Space.Domain.Repositories;

public interface ISpaceRepository
{
    Task<IEnumerable<Domain.Entities.Space>> GetAllAsync(bool includeSouls = false);
    Task<Domain.Entities.Space?> GetByNameAsync(string name, bool includeSouls = false);
    Task<Domain.Entities.Space?> GetByIdAsync(Guid id, bool includeSouls = false);
    Task CreateAsync(Domain.Entities.Space newSpace);
    Task UpdateAsync(Domain.Entities.Space space);
    Task CreateTopicAsync(Topic newTopic);
}
