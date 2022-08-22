﻿using Authentication.Domain.Entities;

namespace Authentication.Domain.Repositories;

public interface IApplicationUserRepository
{
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task RegisterAsync(ApplicationUser applicationUser, string password);
    Task<bool> ValidateRegistrationPassword(string password);
    Task<bool> ValidateLoginPassword(string email, string password);
}
