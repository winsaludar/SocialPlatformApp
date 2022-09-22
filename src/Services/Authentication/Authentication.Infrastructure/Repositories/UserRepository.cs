using Authentication.Core.Contracts;
using Authentication.Infrastructure.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using CoreModels = Authentication.Core.Models;

namespace Authentication.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(UserManager<ApplicationUser> userManager) => _userManager = userManager;

    public async Task<CoreModels.User?> GetByIdAsync(string id)
    {
        var userDb = await _userManager.FindByIdAsync(id);
        if (userDb is null)
            return null;

        var user = userDb.Adapt<CoreModels.User>();
        return user;
    }

    public async Task<CoreModels.User?> GetByEmailAsync(string email)
    {
        var userDb = await _userManager.FindByEmailAsync(email);
        if (userDb is null)
            return null;

        var user = userDb.Adapt<CoreModels.User>();
        return user;
    }

    public async Task<Guid> RegisterAsync(CoreModels.User applicationUser, string password)
    {
        var newUser = applicationUser.Adapt<ApplicationUser>();
        newUser.Id = Guid.NewGuid().ToString();
        newUser.UserName = applicationUser.Email;
        newUser.SecurityStamp = Guid.NewGuid().ToString();

        await _userManager.CreateAsync(newUser, password);

        return Guid.Parse(newUser.Id);
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

