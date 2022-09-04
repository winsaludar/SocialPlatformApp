using Microsoft.EntityFrameworkCore;
using Space.Domain.Entities;
using Space.Domain.Repositories;
using DomainEntities = Space.Domain.Entities;

namespace Space.Persistence.Repositories;

public class SpaceRepository : ISpaceRepository
{
    private readonly SpaceDbContext _dbContext;

    public SpaceRepository(SpaceDbContext dbContext) => _dbContext = dbContext;

    public async Task<IEnumerable<DomainEntities.Space>> GetAllAsync(bool includeSouls = false, bool includeTopics = false)
    {
        IQueryable<DomainEntities.Space> query = _dbContext.Spaces.AsQueryable();

        if (includeSouls)
            query = query.Include(x => x.Members);

        if (includeTopics)
            query = query.Include(x => x.Topics);

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<DomainEntities.Space?> GetByNameAsync(string name, bool includeSouls = false, bool includeTopics = false)
    {
        IQueryable<DomainEntities.Space> query = _dbContext.Spaces.AsQueryable();

        if (includeSouls)
            query = query.Include(x => x.Members);

        if (includeTopics)
            query = query.Include(x => x.Topics);

        return await query.FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<DomainEntities.Space?> GetByIdAsync(Guid id, bool includeSouls = false, bool includeTopics = false)
    {
        IQueryable<DomainEntities.Space> query = _dbContext.Spaces.AsQueryable();

        if (includeSouls)
            query = query.Include(x => x.Members);

        if (includeTopics)
            query = query.Include(x => x.Topics);

        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<DomainEntities.Space?> GetBySlugAsync(string slug, bool includeSouls = false, bool includeTopics = false)
    {
        IQueryable<DomainEntities.Space> query = _dbContext.Spaces.AsQueryable();

        if (includeSouls)
            query = query.Include(x => x.Members);

        if (includeTopics)
            query = query.Include(x => x.Topics);

        return await query.FirstOrDefaultAsync(x => x.Slug.ToLower() == slug.ToLower());
    }

    public async Task CreateAsync(DomainEntities.Space newSpace)
    {
        await _dbContext.Spaces.AddAsync(newSpace);
    }

    public async Task UpdateAsync(DomainEntities.Space space)
    {
        await Task.Run(() => _dbContext.Spaces.Update(space));
    }

    #region TOPICS

    public async Task<Topic?> GetTopicByIdAsync(Guid topicId)
    {
        return await _dbContext.Topics.Where(x => x.Id == topicId).FirstOrDefaultAsync();
    }

    public async Task CreateTopicAsync(Topic newTopic)
    {
        await _dbContext.Topics.AddAsync(newTopic);
    }

    public async Task UpdateTopicAsync(Topic topic)
    {
        await Task.Run(() => _dbContext.Topics.Update(topic));
    }

    public async Task DeleteTopicAsync(Topic topic)
    {
        await Task.Run(() => _dbContext.Topics.Remove(topic));
    }

    #endregion
}
