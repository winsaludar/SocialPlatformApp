namespace Authtentication.Domain.Repositories;

public interface IRepositoryManager
{
    IApplicationUserRepository ApplicationUserRepository { get; }
}
