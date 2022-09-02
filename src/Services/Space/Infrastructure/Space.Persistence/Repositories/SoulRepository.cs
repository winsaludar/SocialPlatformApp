using Microsoft.EntityFrameworkCore;
using Space.Domain.Entities;
using Space.Domain.Repositories;

namespace Space.Persistence.Repositories;

public class SoulRepository : ISoulRepository
{
    private readonly SpaceDbContext _dbContext;

    public SoulRepository(SpaceDbContext dbContext) => _dbContext = dbContext;

    public async Task<Soul?> GetByEmailAsync(string email, bool includeSpaces = false, bool includeTopics = false)
    {
        IQueryable<Soul> query = _dbContext.Souls.AsQueryable();

        if (includeSpaces)
            query = query.Include(x => x.SpacesAsMember);

        if (includeTopics)
            query = query.Include(x => x.Topics);

        return await query.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<Soul?> GetByIdAsync(Guid id, bool includeSpaces = false, bool includeTopics = false)
    {
        IQueryable<Soul> query = _dbContext.Souls.AsQueryable();

        if (includeSpaces)
            query = query.Include(x => x.SpacesAsMember);

        if (includeTopics)
            query = query.Include(x => x.Topics);

        return await query.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task CreateAsync(Soul newSoul)
    {
        await _dbContext.Souls.AddAsync(newSoul);
    }

    public async Task<bool> IsMemberOfSpaceAsync(Guid soulId, Guid spaceId)
    {
        SpaceMember? spaceMember = await _dbContext.SpaceMembers
            .Where(x => x.SoulId == soulId && x.SpaceId == spaceId)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return spaceMember != null;
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
}
