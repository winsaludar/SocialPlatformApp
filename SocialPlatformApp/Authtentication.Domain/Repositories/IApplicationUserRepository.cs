using Authtentication.Domain.Entities;

namespace Authtentication.Domain.Repositories;

public interface IApplicationUserRepository
{
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task RegisterAsync(ApplicationUser applicationUser, string password);
}
