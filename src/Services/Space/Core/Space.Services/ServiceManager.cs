using Space.Domain.Repositories;
using Space.Services.Abstraction;

namespace Space.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<ISpaceService> _lazySpaceService;

    public ServiceManager(IRepositoryManager repositoryManager)
    {
        _lazySpaceService = new Lazy<ISpaceService>(() => new SpaceService(repositoryManager));
    }

    public ISpaceService SpaceService => _lazySpaceService.Value;
}
