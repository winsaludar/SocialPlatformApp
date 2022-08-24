namespace Authentication.Domain.Repositories;

public interface IRepositoryManager
{
    IUserRepository UserRepository { get; }
    ITokenRepository TokenRepository { get; }
    IRefreshTokenRepository RefreshTokenRepository { get; }
}
