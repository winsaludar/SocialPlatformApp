using Authentication.Domain.Exceptions;
using Authentication.Domain.Repositories;
using System.Text.RegularExpressions;

namespace Authentication.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;

    public async Task RegisterAsync(string password, IRepositoryManager repositoryManager)
    {
        if (string.IsNullOrEmpty(Email) || !IsEmailValid(Email))
            throw new InvalidEmailException(Email);

        var existingUser = await repositoryManager.UserRepository.GetByEmailAsync(Email);
        if (existingUser is not null)
            throw new UserAlreadyExistException(Email);

        bool isPasswordValid = await repositoryManager.UserRepository.ValidateRegistrationPasswordAsync(password);
        if (!isPasswordValid)
            throw new InvalidPasswordException();

        await repositoryManager.UserRepository.RegisterAsync(this, password);
    }

    public async Task<Token> LoginAsync(string password, IRepositoryManager repositoryManager)
    {
        if (string.IsNullOrEmpty(Email) || !IsEmailValid(Email))
            throw new InvalidEmailException(Email);

        var existingUser = await repositoryManager.UserRepository.GetByEmailAsync(Email);
        if (existingUser is null)
            throw new UnauthorizedAccessException("Invalid email or password");

        bool isPasswordCorrect = await repositoryManager.UserRepository.ValidateLoginPasswordAsync(Email, password);
        if (!isPasswordCorrect)
            throw new UnauthorizedAccessException("Invalid email or password");

        return await repositoryManager.TokenRepository.GenerateJwtAsync(existingUser);
    }

    private static bool IsEmailValid(string email)
    {
        return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
    }
}
