using Authentication.Core.Contracts;
using Authentication.Core.Exceptions;
using Authentication.Core.Models;
using System.Text.RegularExpressions;

namespace Authentication.Core.Services;

public class AuthService : IAuthService
{
    private readonly IRepositoryManager _repositoryManager;

    public AuthService(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task<Guid> RegisterUserAsync(User newUser, string password)
    {
        if (string.IsNullOrEmpty(newUser.Email) || !IsEmailValid(newUser.Email))
            throw new InvalidEmailException(newUser.Email);

        var existingUser = await _repositoryManager.UserRepository.GetByEmailAsync(newUser.Email);
        if (existingUser is not null)
            throw new UserAlreadyExistException(newUser.Email);

        bool isPasswordValid = await _repositoryManager.UserRepository.ValidateRegistrationPasswordAsync(password);
        if (!isPasswordValid)
            throw new InvalidPasswordException();

        return await _repositoryManager.UserRepository.RegisterAsync(newUser, password);
    }

    public async Task<Token> LoginUserAsync(User user, string password)
    {
        if (string.IsNullOrEmpty(user.Email) || !IsEmailValid(user.Email))
            throw new InvalidEmailException(user.Email);

        var existingUser = await _repositoryManager.UserRepository.GetByEmailAsync(user.Email);
        if (existingUser is null)
            throw new UnauthorizedAccessException("Invalid email or password");

        bool isPasswordCorrect = await _repositoryManager.UserRepository.ValidateLoginPasswordAsync(user.Email, password);
        if (!isPasswordCorrect)
            throw new UnauthorizedAccessException("Invalid email or password");

        return await _repositoryManager.TokenRepository.GenerateJwtAsync(existingUser);
    }

    public async Task<Token> RefreshTokenAsync(Token oldToken)
    {
        var refreshTokenDb = await _repositoryManager.RefreshTokenRepository.GetByOldRefreshTokenAsync(oldToken.RefreshToken);
        if (refreshTokenDb is null)
            throw new InvalidRefreshTokenException();

        var userDb = await _repositoryManager.UserRepository.GetByIdAsync(refreshTokenDb.UserId);
        if (userDb is null)
            throw new InvalidRefreshTokenException();

        try
        {
            return await _repositoryManager.TokenRepository.RefreshJwtAsync(oldToken, userDb, refreshTokenDb);
        }
        catch (Exception)
        {
            throw new InvalidRefreshTokenException();
        }
    }

    private static bool IsEmailValid(string email)
    {
        return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
    }
}

