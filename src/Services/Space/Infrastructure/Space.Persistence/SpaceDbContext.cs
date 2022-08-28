using Microsoft.EntityFrameworkCore;
using Space.Domain.Entities;

namespace Space.Persistence;

public class SpaceDbContext : DbContext
{
    public SpaceDbContext(DbContextOptions<SpaceDbContext> options) : base(options) { }

    public DbSet<Domain.Entities.Space> Spaces { get; set; } = null!;
    public DbSet<Soul> Souls { get; set; } = null!;
    public DbSet<SpaceSoul> SpacesSouls { get; set; } = null!;
    public DbSet<Topic> Topics { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SpaceDbContext).Assembly);
    }
}
