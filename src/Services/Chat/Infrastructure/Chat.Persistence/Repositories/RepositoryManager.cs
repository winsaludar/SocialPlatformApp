using Chat.Domain.Repositories;

namespace Chat.Persistence.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly Lazy<IUnitOfWork> _lazyUnitOfWork;

    public RepositoryManager()
    {
        _lazyUnitOfWork = new Lazy<IUnitOfWork>(() => new UnitOfWork());
    }

    public IUnitOfWork UnitOfWork => _lazyUnitOfWork.Value;
}
