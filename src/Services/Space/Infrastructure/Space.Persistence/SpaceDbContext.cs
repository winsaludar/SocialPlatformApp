using Microsoft.EntityFrameworkCore;
using DomainEntities = Space.Domain.Entities;

namespace Space.Persistence;

public class SpaceDbContext : DbContext
{
    public SpaceDbContext(DbContextOptions<SpaceDbContext> options) : base(options) { }

    public DbSet<DomainEntities.Space> Spaces { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SpaceDbContext).Assembly);
    }
}
