namespace Authentication.Services.Abstraction;

public interface IServiceManager
{
    IAuthenticationService AuthenticationService { get; }
}
