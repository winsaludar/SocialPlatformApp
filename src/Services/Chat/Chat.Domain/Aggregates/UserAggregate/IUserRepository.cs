using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.UserAggregate;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<Guid> AddAsync(User newUser);
}
