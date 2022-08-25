using Microsoft.EntityFrameworkCore;
using Space.Domain.Entities;
using Space.Domain.Repositories;

namespace Space.Persistence.Repositories;

public class SoulRepository : ISoulRepository
{
    private readonly SpaceDbContext _dbContext;

    public SoulRepository(SpaceDbContext dbContext) => _dbContext = dbContext;

    public async Task<Soul?> GetByEmailAsync(string email)
    {
        return await _dbContext.Souls.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task CreateAsync(Soul newSoul)
    {
        await _dbContext.Souls.AddAsync(newSoul);
    }

    public async Task<bool> IsMemberOfSpace(Guid soulId, Guid spaceId)
    {
        Domain.Entities.Space? space = await _dbContext.Spaces.Where(x => x.Id == spaceId).AsNoTracking().FirstOrDefaultAsync();
        if (space == null)
            return false;

        Soul? soul = await _dbContext.Souls.Include(x => x.Spaces)
            .Where(x => x.Id == soulId && x.Spaces.Contains(space))
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return soul != null;
    }
}
