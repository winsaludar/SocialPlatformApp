namespace Chat.Domain.Repositories;

public interface IRepositoryManager
{
    IUnitOfWork UnitOfWork { get; }
}
