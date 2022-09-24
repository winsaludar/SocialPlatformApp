namespace Chat.Domain.Aggregates.UserAggregate;

public interface IUserRepository
{
    Task<Guid> AddAsync(User newUser);
}
