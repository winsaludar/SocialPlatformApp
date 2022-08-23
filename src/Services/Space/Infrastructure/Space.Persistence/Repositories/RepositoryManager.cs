using Space.Domain.Repositories;

namespace Space.Persistence.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly Lazy<ISpaceRepository> _lazySpaceRepository;

    public RepositoryManager(SpaceDbContext dbContext)
    {
        _lazySpaceRepository = new Lazy<ISpaceRepository>(() => new SpaceRepository(dbContext));
    }

    public ISpaceRepository SpaceRepository => _lazySpaceRepository.Value;
}
