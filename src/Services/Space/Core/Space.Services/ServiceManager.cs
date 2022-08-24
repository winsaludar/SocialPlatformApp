using Space.Domain.Repositories;
using Space.Services.Abstraction;

namespace Space.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<ISpaceService> _lazySpaceService;
    private readonly Lazy<ISoulService> _lazySoulService;

    public ServiceManager(IRepositoryManager repositoryManager)
    {
        _lazySpaceService = new Lazy<ISpaceService>(() => new SpaceService(repositoryManager));
        _lazySoulService = new Lazy<ISoulService>(() => new SoulService(repositoryManager));
    }

    public ISpaceService SpaceService => _lazySpaceService.Value;
    public ISoulService SoulService => _lazySoulService.Value;
}
