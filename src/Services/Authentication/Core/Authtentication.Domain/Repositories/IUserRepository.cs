using Authentication.Domain.Entities;

namespace Authentication.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task<Guid> RegisterAsync(User applicationUser, string password);
    Task<bool> ValidateRegistrationPasswordAsync(string password);
    Task<bool> ValidateLoginPasswordAsync(string email, string password);
}
