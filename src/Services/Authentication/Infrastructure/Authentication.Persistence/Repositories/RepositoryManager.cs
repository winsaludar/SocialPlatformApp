using Authentication.Domain.Repositories;
using Authentication.Persistence.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.Persistence.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly Lazy<IUserRepository> _lazyApplicationUserRepository;
    private readonly Lazy<IRefreshTokenRepository> _lazyRefreshTokenRepository;
    private readonly Lazy<ITokenRepository> _lazyTokenRepository;

    public RepositoryManager(
        UserManager<ApplicationUser> userManager,
        AuthenticationDbContext dbContext,
        TokenValidationParameters tokenValidationParameters,
        IConfiguration configuration)
    {
        _lazyApplicationUserRepository = new Lazy<IUserRepository>(() => new UserRepository(userManager));
        _lazyRefreshTokenRepository = new Lazy<IRefreshTokenRepository>(() => new RefreshTokenRepository(dbContext));
        _lazyTokenRepository = new Lazy<ITokenRepository>(() => new TokenRepository(tokenValidationParameters, configuration, _lazyRefreshTokenRepository.Value));
    }

    public IUserRepository ApplicationUserRepository => _lazyApplicationUserRepository.Value;
    public IRefreshTokenRepository RefreshTokenRepository => _lazyRefreshTokenRepository.Value;
    public ITokenRepository TokenRepository => _lazyTokenRepository.Value;
}
