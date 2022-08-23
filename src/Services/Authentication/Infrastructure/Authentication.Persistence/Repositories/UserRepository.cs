using Authentication.Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Identity;
using DomainEntities = Authentication.Domain.Entities;
using PersistenceModels = Authentication.Persistence.Models;

namespace Authentication.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<PersistenceModels.ApplicationUser> _userManager;

    public UserRepository(UserManager<PersistenceModels.ApplicationUser> userManager) => _userManager = userManager;

    public async Task<DomainEntities.User?> GetByIdAsync(string id)
    {
        var userDb = await _userManager.FindByIdAsync(id);
        if (userDb is null)
            return null;

        var user = userDb.Adapt<DomainEntities.User>();
        return user;
    }

    public async Task<DomainEntities.User?> GetByEmailAsync(string email)
    {
        var userDb = await _userManager.FindByEmailAsync(email);
        if (userDb is null)
            return null;

        var user = userDb.Adapt<DomainEntities.User>();
        return user;
    }

    public async Task RegisterAsync(DomainEntities.User applicationUser, string password)
    {
        var newUser = applicationUser.Adapt<PersistenceModels.ApplicationUser>();
        newUser.Id = Guid.NewGuid().ToString();
        newUser.UserName = applicationUser.Email;
        newUser.SecurityStamp = Guid.NewGuid().ToString();

        await _userManager.CreateAsync(newUser, password);
    }

    public async Task<bool> ValidateRegistrationPasswordAsync(string password)
    {
        var passwordValidators = _userManager.PasswordValidators;
        foreach (var validator in passwordValidators)
        {
            var result = await validator.ValidateAsync(_userManager, null!, password);
            if (!result.Succeeded)
                return false;
        }

        return true;
    }

    public async Task<bool> ValidateLoginPasswordAsync(string email, string password)
    {
        var userDb = await _userManager.FindByEmailAsync(email);
        if (userDb is null)
            return false;

        return await _userManager.CheckPasswordAsync(userDb, password);
    }
}
