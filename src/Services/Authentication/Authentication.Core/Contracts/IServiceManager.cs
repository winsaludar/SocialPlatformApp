namespace Authentication.Core.Contracts;

public interface IServiceManager
{
    IAuthenticationService AuthenticationService { get; }
}
