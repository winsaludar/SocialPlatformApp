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
    private readonly ITokenService _tokenService;

    public ApplicationUserService(IRepositoryManager repositoryManager, ITokenService tokenService)
    {
        _repositoryManager = repositoryManager;
        _tokenService = tokenService;
    }

    public async Task<UserDto> GetByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email) || !IsEmailValid(email))
            throw new InvalidEmailException(email);

        var user = await _repositoryManager.ApplicationUserRepository.GetByEmailAsync(email);
        if (user is null)
            throw new UserNotFoundException(email);

        var userDto = user.Adapt<UserDto>();
        return userDto;
    }

    public async Task RegisterAsync(RegisterUserDto registerApplicationUserDto)
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

    public async Task<TokenDto> LoginAsync(LoginUserDto loginUserDto)
    {
        if (string.IsNullOrEmpty(loginUserDto.Email) || !IsEmailValid(loginUserDto.Email))
            throw new InvalidEmailException(loginUserDto.Email);

        var existingUser = await _repositoryManager.ApplicationUserRepository.GetByEmailAsync(loginUserDto.Email);
        if (existingUser is null)
            throw new UnauthorizedAccessException("Invalid email or password");

        bool isPasswordCorrect = await _repositoryManager.ApplicationUserRepository.ValidateLoginPassword(loginUserDto.Email, loginUserDto.Password);
        if (!isPasswordCorrect)
            throw new UnauthorizedAccessException("Invalid email or password");

        var user = existingUser.Adapt<UserDto>();
        return await _tokenService.GenerateJwtAsync(user);
    }

    private static bool IsEmailValid(string email)
    {
        return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
    }
}
