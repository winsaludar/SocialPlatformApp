using Authentication.Domain.Repositories;
using Authentication.Services.Abstraction;

namespace Authentication.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IApplicationUserService> _lazyApplicationUserService;

    public ServiceManager(IRepositoryManager repositoryManager)
    {
        _lazyApplicationUserService = new Lazy<IApplicationUserService>(() => new ApplicationUserService(repositoryManager));
    }

    public IApplicationUserService ApplicationUserService => _lazyApplicationUserService.Value;
}
