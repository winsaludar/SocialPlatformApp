using Authentication.Domain.Repositories;
using Authentication.Services.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IApplicationUserService> _lazyApplicationUserService;
    private readonly Lazy<ITokenService> _lazyTokenService;

    public ServiceManager(IRepositoryManager repositoryManager, IConfiguration configuration, TokenValidationParameters tokenValidationParameters, ITokenService tokenService)
    {
        _lazyApplicationUserService = new Lazy<IApplicationUserService>(() => new ApplicationUserService(repositoryManager, tokenService));
        _lazyTokenService = new Lazy<ITokenService>(() => new TokenService(repositoryManager, configuration, tokenValidationParameters));
    }

    public IApplicationUserService ApplicationUserService => _lazyApplicationUserService.Value;
    public ITokenService TokenService => _lazyTokenService.Value;
}
