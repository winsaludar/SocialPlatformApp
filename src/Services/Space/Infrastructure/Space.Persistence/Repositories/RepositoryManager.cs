using Space.Domain.Repositories;

namespace Space.Persistence.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly Lazy<IUnitOfWork> _lazyUnitOfWork;
    private readonly Lazy<ISpaceRepository> _lazySpaceRepository;

    public RepositoryManager(SpaceDbContext dbContext)
    {
        _lazyUnitOfWork = new Lazy<IUnitOfWork>(() => new UnitOfWork(dbContext));
        _lazySpaceRepository = new Lazy<ISpaceRepository>(() => new SpaceRepository(dbContext));
    }

    public IUnitOfWork UnitOfWork => _lazyUnitOfWork.Value;
    public ISpaceRepository SpaceRepository => _lazySpaceRepository.Value;
}
