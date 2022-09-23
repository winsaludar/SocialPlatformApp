namespace Authentication.Core.Contracts;

public interface IServiceManager
{
    IAuthService AuthenticationService { get; }
}
