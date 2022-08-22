using Authentication.Domain.Repositories;
using Authentication.Services.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IApplicationUserService> _lazyApplicationUserService;

    public ServiceManager(IRepositoryManager repositoryManager, IConfiguration configuration, TokenValidationParameters tokenValidationParameters)
    {
        _lazyApplicationUserService = new Lazy<IApplicationUserService>(() => new ApplicationUserService(repositoryManager, configuration, tokenValidationParameters));
    }

    public IApplicationUserService ApplicationUserService => _lazyApplicationUserService.Value;
}
