using Authentication.Contracts;
using Authentication.Domain.Entities;
using Authentication.Domain.Exceptions;
using Authentication.Domain.Repositories;
using Authentication.Services.Abstraction;
using Mapster;

namespace Authentication.Services;

internal class ApplicationUserService : IApplicationUserService
{
    private readonly IRepositoryManager _repositoryManager;

    public ApplicationUserService(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<ApplicationUserDto> GetByEmailAsync(string email)
    {
        var user = await _repositoryManager.ApplicationUserRepository.GetByEmailAsync(email);
        if (user is null)
            throw new UserNotFoundException(email);

        var userDto = user.Adapt<ApplicationUserDto>();
        return userDto;
    }

    public async Task RegisterAsync(RegisterApplicationUserDto registerApplicationUserDto)
    {
        var existingUser = await _repositoryManager.ApplicationUserRepository.GetByEmailAsync(registerApplicationUserDto.Email);
        if (existingUser is not null)
            throw new UserAlreadyExistException(registerApplicationUserDto.Email);

        ApplicationUser newUser = new()
        {
            FirstName = registerApplicationUserDto.FirstName,
            LastName = registerApplicationUserDto.LastName,
            Email = registerApplicationUserDto.Email
        };

        await _repositoryManager.ApplicationUserRepository.RegisterAsync(newUser, registerApplicationUserDto.Password);
    }
}
