namespace Authentication.Domain.Repositories;

public interface IRepositoryManager
{
    IUserRepository ApplicationUserRepository { get; }
    ITokenRepository TokenRepository { get; }
    IRefreshTokenRepository RefreshTokenRepository { get; }
}
