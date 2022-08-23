namespace Space.Domain.Repositories;

public interface IRepositoryManager
{
    ISpaceRepository SpaceRepository { get; }
}
