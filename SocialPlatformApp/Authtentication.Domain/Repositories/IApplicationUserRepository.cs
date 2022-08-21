using Authentication.Domain.Entities;

namespace Authentication.Domain.Repositories;

public interface IApplicationUserRepository
{
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task RegisterAsync(ApplicationUser applicationUser, string password);
}
