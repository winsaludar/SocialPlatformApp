namespace Chat.Domain.Aggregates.UserAggregate;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<Guid> AddAsync(User newUser);
}
