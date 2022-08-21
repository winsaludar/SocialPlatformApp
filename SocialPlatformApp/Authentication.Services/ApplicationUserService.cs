using Authentication.Contracts;
using Authentication.Domain.Entities;
using Authentication.Domain.Exceptions;
using Authentication.Domain.Repositories;
using Authentication.Services.Abstraction;
using Mapster;
using System.Text.RegularExpressions;

namespace Authentication.Services;

public class ApplicationUserService : IApplicationUserService
{
    private readonly IRepositoryManager _repositoryManager;

    public ApplicationUserService(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<ApplicationUserDto> GetByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email) || !IsEmailValid(email))
            throw new InvalidEmailException(email);

        var user = await _repositoryManager.ApplicationUserRepository.GetByEmailAsync(email);
        if (user is null)
            throw new UserNotFoundException(email);

        var userDto = user.Adapt<ApplicationUserDto>();
        return userDto;
    }

    public async Task RegisterAsync(RegisterApplicationUserDto registerApplicationUserDto)
    {
        if (string.IsNullOrEmpty(registerApplicationUserDto.Email) || !IsEmailValid(registerApplicationUserDto.Email))
            throw new InvalidEmailException(registerApplicationUserDto.Email);

        var existingUser = await _repositoryManager.ApplicationUserRepository.GetByEmailAsync(registerApplicationUserDto.Email);
        if (existingUser is not null)
            throw new UserAlreadyExistException(registerApplicationUserDto.Email);

        bool isPasswordValid = await _repositoryManager.ApplicationUserRepository.ValidateRegistrationPassword(registerApplicationUserDto.Password);
        if (!isPasswordValid)
            throw new InvalidPasswordException();

        ApplicationUser newUser = new()
        {
            FirstName = registerApplicationUserDto.FirstName,
            LastName = registerApplicationUserDto.LastName,
            Email = registerApplicationUserDto.Email
        };

        await _repositoryManager.ApplicationUserRepository.RegisterAsync(newUser, registerApplicationUserDto.Password);
    }

    private static bool IsEmailValid(string email)
    {
        return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
    }
}
