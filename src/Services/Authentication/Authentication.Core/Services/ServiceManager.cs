using Authentication.Core.Contracts;

namespace Authentication.Core.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IAuthService> _lazyAuthenticationService;

    public ServiceManager(IRepositoryManager repositoryManager)
    {
        _lazyAuthenticationService = new Lazy<IAuthService>(() => new AuthService(repositoryManager));
    }

    public IAuthService AuthenticationService => _lazyAuthenticationService.Value;
}

