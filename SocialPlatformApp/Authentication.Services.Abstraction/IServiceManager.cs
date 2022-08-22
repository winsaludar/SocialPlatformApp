namespace Authentication.Services.Abstraction;

public interface IServiceManager
{
    IApplicationUserService ApplicationUserService { get; }
    ITokenService TokenService { get; }
}
