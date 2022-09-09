using Microsoft.EntityFrameworkCore;
using Space.Domain.Entities;
using Space.Domain.Repositories;

namespace Space.Persistence.Repositories;

public class SoulRepository : ISoulRepository
{
    private readonly SpaceDbContext _dbContext;

    public SoulRepository(SpaceDbContext dbContext) => _dbContext = dbContext;

    public async Task<Soul?> GetByEmailAsync(
        string email,
        bool includeMemberSpaces = false,
        bool includeTopics = false,
        bool includeModeratedSpaces = false)
    {
        IQueryable<Soul> query = _dbContext.Souls.AsQueryable();

        if (includeMemberSpaces)
            query = query.Include(x => x.SpacesAsMember);

        if (includeTopics)
            query = query.Include(x => x.Topics);

        if (includeModeratedSpaces)
            query = query.Include(x => x.SpacesAsModerator);

        return await query.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<Soul?> GetByIdAsync(
        Guid id,
        bool includeMemberSpaces = false,
        bool includeTopics = false,
        bool includeModeratedSpaces = false)
    {
        IQueryable<Soul> query = _dbContext.Souls.AsQueryable();

        if (includeModeratedSpaces)
            query = query.Include(x => x.SpacesAsMember);

        if (includeTopics)
            query = query.Include(x => x.Topics);

        if (includeModeratedSpaces)
            query = query.Include(x => x.SpacesAsModerator);

        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task CreateAsync(Soul newSoul)
    {
        await _dbContext.Souls.AddAsync(newSoul);
    }

    public async Task UpdateAsync(Soul soul)
    {
        await Task.Run(() => _dbContext.Souls.Update(soul));
    }

    public async Task<bool> IsMemberOfSpaceAsync(Guid soulId, Guid spaceId)
    {
        SpaceMember? spaceMember = await _dbContext.SpaceMembers
            .Where(x => x.SoulId == soulId && x.SpaceId == spaceId)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return spaceMember != null;
    }

    public async Task<bool> IsModeratorOfSpaceAsync(Guid soulId, Guid spaceId)
    {
        SpaceModerator? spaceModerator = await _dbContext.SpaceModerators
            .Where(x => x.SoulId == soulId && x.SpaceId == spaceId)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return spaceModerator != null;
    }

    public async Task DeleteSpaceMemberAsync(Guid soulId, Guid spaceId)
    {
        await Task.Run(() =>
        {
            SpaceMember spaceMember = new() { SoulId = soulId, SpaceId = spaceId };
            _dbContext.SpaceMembers.Attach(spaceMember);
            _dbContext.SpaceMembers.Remove(spaceMember);
        });
    }

    public async Task<SoulTopicVote?> GetTopicVoteAsync(Guid soulId, Guid topicId)
    {
        return await _dbContext.SoulTopicVotes.FirstOrDefaultAsync(x => x.SoulId == soulId && x.TopicId == topicId);
    }

    public async Task CreateTopicVoteAsync(SoulTopicVote newVote)
    {
        await _dbContext.SoulTopicVotes.AddAsync(newVote);
    }

    public async Task UpdateTopicVoteAsync(SoulTopicVote vote)
    {
        await Task.Run(() => _dbContext.SoulTopicVotes.Update(vote));
    }
}
