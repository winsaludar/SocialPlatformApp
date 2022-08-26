using Microsoft.EntityFrameworkCore;
using Space.Domain.Repositories;
using DomainEntities = Space.Domain.Entities;

namespace Space.Persistence.Repositories;

public class SpaceRepository : ISpaceRepository
{
    private readonly SpaceDbContext _dbContext;

    public SpaceRepository(SpaceDbContext dbContext) => _dbContext = dbContext;

    public async Task<IEnumerable<DomainEntities.Space>> GetAllAsync(bool includeSouls = false)
    {
        IQueryable<DomainEntities.Space> query = _dbContext.Spaces.AsQueryable();
        if (includeSouls)
            query = query.Include(x => x.Souls);

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<DomainEntities.Space?> GetByNameAsync(string name, bool includeSouls = false)
    {
        IQueryable<DomainEntities.Space> query = _dbContext.Spaces.AsQueryable();
        if (includeSouls)
            query = query.Include(x => x.Souls);

        return await query.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<DomainEntities.Space?> GetByIdAsync(Guid id, bool includeSouls = false)
    {
        IQueryable<DomainEntities.Space> query = _dbContext.Spaces.AsQueryable();
        if (includeSouls)
            query = query.Include(x => x.Souls);

        return await query.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task CreateAsync(DomainEntities.Space newSpace)
    {
        await _dbContext.Spaces.AddAsync(newSpace);
    }

    public async Task UpdateAsync(DomainEntities.Space space)
    {
        await Task.Run(() => _dbContext.Spaces.Update(space));
    }
}
