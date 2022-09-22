using Authentication.Core.Contracts;
using Authentication.Core.Exceptions;
using System.Text.RegularExpressions;

namespace Authentication.Core.Models;

public class User
{
    private readonly IRepositoryManager? _repositoryManager;

    public User() { }

    public User(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;

    public async Task<Guid> RegisterAsync(string password)
    {
        if (_repositoryManager == null)
            throw new NullReferenceException("IRepositoryManager is null");

        if (string.IsNullOrEmpty(Email) || !IsEmailValid(Email))
            throw new InvalidEmailException(Email);

        var existingUser = await _repositoryManager.UserRepository.GetByEmailAsync(Email);
        if (existingUser is not null)
            throw new UserAlreadyExistException(Email);

        bool isPasswordValid = await _repositoryManager.UserRepository.ValidateRegistrationPasswordAsync(password);
        if (!isPasswordValid)
            throw new InvalidPasswordException();

        return await _repositoryManager.UserRepository.RegisterAsync(this, password);
    }

    public async Task<Token> LoginAsync(string password)
    {
        if (_repositoryManager == null)
            throw new NullReferenceException("IRepositoryManager is null");

        if (string.IsNullOrEmpty(Email) || !IsEmailValid(Email))
            throw new InvalidEmailException(Email);

        var existingUser = await _repositoryManager.UserRepository.GetByEmailAsync(Email);
        if (existingUser is null)
            throw new UnauthorizedAccessException("Invalid email or password");

        bool isPasswordCorrect = await _repositoryManager.UserRepository.ValidateLoginPasswordAsync(Email, password);
        if (!isPasswordCorrect)
            throw new UnauthorizedAccessException("Invalid email or password");

        return await _repositoryManager.TokenRepository.GenerateJwtAsync(existingUser);
    }

    private static bool IsEmailValid(string email)
    {
        return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
    }
}
