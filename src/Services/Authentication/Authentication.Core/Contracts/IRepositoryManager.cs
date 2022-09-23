namespace Authentication.Core.Contracts;

public interface IRepositoryManager
{
    IUserRepository UserRepository { get; }
    ITokenRepository TokenRepository { get; }
    IRefreshTokenRepository RefreshTokenRepository { get; }
}
