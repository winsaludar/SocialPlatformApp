using Authentication.Domain.Entities;

namespace Authentication.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task RegisterAsync(User applicationUser, string password);
    Task<bool> ValidateRegistrationPassword(string password);
    Task<bool> ValidateLoginPassword(string email, string password);
}
