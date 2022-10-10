namespace Chat.Domain.SeedWork;

public interface IUserManager
{
    public Task<Guid> GetUserIdByEmailAsync(string email);
}
