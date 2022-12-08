using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;

namespace Chat.Domain.Aggregates.UserAggregate;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task<Guid> AddAsync(User newUser);
    Task<IEnumerable<Server>> GetUserServersAsync(Guid id);
}
