namespace Chat.Domain.SeedWork;

public class UserManager : IUserManager
{
    private readonly IRepositoryManager _repositoryManager;

    public UserManager(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<Guid> GetUserIdByEmailAsync(string email)
    {
        // There is a possibility that a user does not have any data in the database.
        // This happens when the integration event handler that registers the user throws an error.
        // We will return an empty Guid just to proceed, later we will handle all servers that does have empty createdById

        if (string.IsNullOrEmpty(email))
            return Guid.Empty;

        var result = await _repositoryManager.UserRepository.GetByEmailAsync(email);
        if (result is null)
            return Guid.Empty;

        return result.Id;
    }
}
