using Space.Domain.Repositories;
using DomainEntities = Space.Domain.Entities;

namespace Space.Persistence.Repositories;

internal sealed class SpaceRepository : ISpaceRepository
{
    private readonly SpaceDbContext _dbContext;

    public SpaceRepository(SpaceDbContext dbContext) => _dbContext = dbContext;

    public async Task CreateAsync(DomainEntities.Space newSpace)
    {
        await _dbContext.Spaces.AddAsync(newSpace);
        await _dbContext.SaveChangesAsync();
    }
}
