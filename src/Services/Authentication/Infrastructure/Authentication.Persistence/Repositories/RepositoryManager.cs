using Authentication.Domain.Repositories;
using Authentication.Persistence.Models;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Persistence.Repositories;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly Lazy<IApplicationUserRepository> _applicationUserRepository;
    private readonly Lazy<IRefreshTokenRepository> _refreshTokenRepository;

    public RepositoryManager(UserManager<ApplicationUser> userManager, AuthenticationDbContext dbContext)
    {
        _applicationUserRepository = new Lazy<IApplicationUserRepository>(() => new ApplicationUserRepository(userManager));
        _refreshTokenRepository = new Lazy<IRefreshTokenRepository>(() => new RefreshTokenRepository(dbContext));
    }

    public IApplicationUserRepository ApplicationUserRepository => _applicationUserRepository.Value;
    public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository.Value;
}
