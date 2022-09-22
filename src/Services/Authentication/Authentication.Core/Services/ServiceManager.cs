using Authentication.Core.Contracts;

namespace Authentication.Core.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IAuthenticationService> _lazyAuthenticationService;

    public ServiceManager(IRepositoryManager repositoryManager)
    {
        _lazyAuthenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(repositoryManager));
    }

    public IAuthenticationService AuthenticationService => _lazyAuthenticationService.Value;
}

