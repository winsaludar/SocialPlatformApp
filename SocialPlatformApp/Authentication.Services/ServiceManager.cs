using Authentication.Domain.Repositories;
using Authentication.Services.Abstraction;
using Microsoft.Extensions.Configuration;

namespace Authentication.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IApplicationUserService> _lazyApplicationUserService;

    public ServiceManager(IRepositoryManager repositoryManager, IConfiguration configuration)
    {
        _lazyApplicationUserService = new Lazy<IApplicationUserService>(() => new ApplicationUserService(repositoryManager, configuration));
    }

    public IApplicationUserService ApplicationUserService => _lazyApplicationUserService.Value;
}
