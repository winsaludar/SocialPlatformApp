using Microsoft.EntityFrameworkCore;
using Space.Domain.Entities;
using Space.Domain.Repositories;
using DomainEntities = Space.Domain.Entities;

namespace Space.Persistence.Repositories;

public class SpaceRepository : ISpaceRepository
{
    private readonly SpaceDbContext _dbContext;

    public SpaceRepository(SpaceDbContext dbContext) => _dbContext = dbContext;

    public async Task<IEnumerable<DomainEntities.Space>> GetAllAsync(bool includeMembers = false, bool includeTopics = false, bool includeModerators = false)
    {
        IQueryable<DomainEntities.Space> query = _dbContext.Spaces.AsQueryable();

        if (includeMembers)
            query = query.Include(x => x.Members);

        if (includeTopics)
            query = query.Include(x => x.Topics);

        if (includeModerators)
            query = query.Include(x => x.Moderators);

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<DomainEntities.Space?> GetByNameAsync(string name, bool includeMembers = false, bool includeTopics = false, bool includeModerators = false)
    {
        IQueryable<DomainEntities.Space> query = _dbContext.Spaces.AsQueryable();

        if (includeMembers)
            query = query.Include(x => x.Members);

        if (includeTopics)
            query = query.Include(x => x.Topics);

        if (includeModerators)
            query = query.Include(x => x.Moderators);

        return await query.FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<DomainEntities.Space?> GetByIdAsync(Guid id, bool includeMembers = false, bool includeTopics = false, bool includeModerators = false)
    {
        IQueryable<DomainEntities.Space> query = _dbContext.Spaces.AsQueryable();

        if (includeMembers)
            query = query.Include(x => x.Members);

        if (includeTopics)
            query = query.Include(x => x.Topics);

        if (includeModerators)
            query = query.Include(x => x.Moderators);

        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<DomainEntities.Space?> GetBySlugAsync(string slug, bool includeMembers = false, bool includeTopics = false, bool includeModerators = false)
    {
        IQueryable<DomainEntities.Space> query = _dbContext.Spaces.AsQueryable();

        if (includeMembers)
            query = query.Include(x => x.Members);

        if (includeTopics)
            query = query.Include(x => x.Topics);

        if (includeModerators)
            query = query.Include(x => x.Moderators);

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

    public async Task<Topic?> GetTopicByIdAsync(Guid topicId)
    {
        return await _dbContext.Topics.Where(x => x.Id == topicId).FirstOrDefaultAsync();
    }

    public async Task<Topic?> GetTopicBySlugAsync(string topicSlug)
    {
        return await _dbContext.Topics.Where(x => x.Slug == topicSlug).FirstOrDefaultAsync();
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

    public async Task<(int upvotes, int downvotes)> GetTopicVotesAsync(Guid topicId)
    {
        var result = await (from vote in _dbContext.SoulTopicVotes
                            where vote.TopicId == topicId
                            group vote by vote.TopicId into g
                            select new
                            {
                                Upvotes = g.Sum(x => x.Upvote),
                                Downvotes = g.Sum(x => x.Downvote)
                            }).FirstOrDefaultAsync();

        if (result == null)
            return (0, 0);

        return (result.Upvotes, result.Downvotes);
    }

    public async Task CreateCommentAsync(Comment newComment)
    {
        await _dbContext.Comments.AddAsync(newComment);
    }
}
