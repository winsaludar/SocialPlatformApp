﻿using Authentication.Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Identity;
using DomainEntities = Authentication.Domain.Entities;
using PersistenceModels = Authentication.Persistence.Models;

namespace Authentication.Persistence.Repositories;

internal sealed class ApplicationUserRepository : IApplicationUserRepository
{
    private readonly UserManager<PersistenceModels.ApplicationUser> _userManager;

    public ApplicationUserRepository(UserManager<PersistenceModels.ApplicationUser> userManager) => _userManager = userManager;

    public async Task<DomainEntities.ApplicationUser?> GetByEmailAsync(string email)
    {
        var userDb = await _userManager.FindByEmailAsync(email);
        if (userDb is null)
            return null;

        var user = userDb.Adapt<DomainEntities.ApplicationUser>();
        return user;
    }

    public async Task RegisterAsync(DomainEntities.ApplicationUser applicationUser, string password)
    {
        var newUser = applicationUser.Adapt<PersistenceModels.ApplicationUser>();
        newUser.Id = Guid.NewGuid().ToString();
        newUser.UserName = applicationUser.Email;
        newUser.SecurityStamp = Guid.NewGuid().ToString();

        await _userManager.CreateAsync(newUser, password);
    }
}
