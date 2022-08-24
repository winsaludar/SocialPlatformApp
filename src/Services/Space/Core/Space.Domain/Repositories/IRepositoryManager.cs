namespace Space.Domain.Repositories;

public interface IRepositoryManager
{
    IUnitOfWork UnitOfWork { get; }
    ISpaceRepository SpaceRepository { get; }
}
