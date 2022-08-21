using Authentication.Domain.Repositories;
using Authentication.Persistence.Models;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Persistence.Repositories;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly Lazy<IApplicationUserRepository> _applicationUserRepository;

    public RepositoryManager(UserManager<ApplicationUser> userManager)
    {
        _applicationUserRepository = new Lazy<IApplicationUserRepository>(() => new ApplicationUserRepository(userManager));
    }

    public IApplicationUserRepository ApplicationUserRepository => _applicationUserRepository.Value;
}
