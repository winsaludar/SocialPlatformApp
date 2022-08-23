using Microsoft.EntityFrameworkCore;
using Space.Domain.Repositories;
using DomainEntities = Space.Domain.Entities;

namespace Space.Persistence.Repositories;

public class SpaceRepository : ISpaceRepository
{
    private readonly SpaceDbContext _dbContext;

    public SpaceRepository(SpaceDbContext dbContext) => _dbContext = dbContext;

    public async Task<DomainEntities.Space?> GetByNameAsync(string name)
    {
        return await _dbContext.Spaces.FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task CreateAsync(DomainEntities.Space newSpace)
    {
        await _dbContext.Spaces.AddAsync(newSpace);
        await _dbContext.SaveChangesAsync();
    }
}
