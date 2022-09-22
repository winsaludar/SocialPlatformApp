using Authentication.Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Infrastructure;

public class AuthenticationDbContext : IdentityDbContext<ApplicationUser>
{
    public AuthenticationDbContext(DbContextOptions options) : base(options) { }

    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthenticationDbContext).Assembly);
    }
}

