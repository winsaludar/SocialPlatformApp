namespace Authentication.Domain.Repositories;

public interface IRepositoryManager
{
    IApplicationUserRepository ApplicationUserRepository { get; }
    IRefreshTokenRepository RefreshTokenRepository { get; }
}
