using Space.Domain.Entities;

namespace Space.Domain.Repositories;

public interface ISpaceRepository
{
    Task<IEnumerable<Entities.Space>> GetAllAsync(bool includeSouls = false, bool includeTopics = false);
    Task<Entities.Space?> GetByNameAsync(string name, bool includeSouls = false, bool includeTopics = false);
    Task<Entities.Space?> GetByIdAsync(Guid id, bool includeSouls = false, bool includeTopics = false);
    Task CreateAsync(Entities.Space newSpace);
    Task UpdateAsync(Entities.Space space);

    Task<Topic?> GetTopicByIdAsync(Guid topicId);
    Task CreateTopicAsync(Topic newTopic);
    Task UpdateTopicAsync(Topic topic);
    Task DeleteTopicAsync(Topic topic);
}
