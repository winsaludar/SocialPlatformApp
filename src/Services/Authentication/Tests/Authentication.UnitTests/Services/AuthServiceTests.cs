using Authentication.Core.Contracts;
using Authentication.Core.Exceptions;
using Authentication.Core.Models;
using Authentication.Core.Services;
using Moq;

namespace Authentication.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        Mock<IUserRepository> mockUserRepository = new();
        Mock<ITokenRepository> mockTokenRepository = new();
        Mock<IRefreshTokenRepository> mockRefreshTokenRepository = new();

        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.UserRepository).Returns(mockUserRepository.Object);
        _mockRepositoryManager.Setup(x => x.TokenRepository).Returns(mockTokenRepository.Object);
        _mockRepositoryManager.Setup(x => x.RefreshTokenRepository).Returns(mockRefreshTokenRepository.Object);

        _authService = new AuthService(_mockRepositoryManager.Object);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("testemail.com")]
    [InlineData("test@emailcom")]
    public async Task RegisterUserAsync_EmailIsInvalid_ThrowsInvalidEmailException(string email)
    {
        // Arrange
        User newUser = new("First", "Last", email, null);
        string password = "password";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidEmailException>(() => _authService.RegisterUserAsync(newUser, password));
    }

    [Fact]
    public async Task RegisterUserAsync_EmailAlreadyExist_ThrowsUserAlreadyExistException()
    {
        // Arrange
        User newUser = new("First", "Last", "existinguser@email.com", null);
        string password = "password";
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new User());

        // Act & Assert
        await Assert.ThrowsAsync<UserAlreadyExistException>(() => _authService.RegisterUserAsync(newUser, password));
    }

    [Fact]
    public async Task RegisterUserAsync_PasswordIsInvalid_ThrowsInvalidPasswordException()
    {
        // Arrange
        User newUser = new("First", "Last", "nonexistinguser@email.com", null);
        string password = "";
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null!);
        _mockRepositoryManager.Setup(x => x.UserRepository.ValidateRegistrationPasswordAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidPasswordException>(() => _authService.RegisterUserAsync(newUser, password));
    }

    [Fact]
    public async Task RegisterUserAsync_EmailAndPasswordAreBothValid_CreateUser()
    {
        // Arrange
        User newUser = new("First", "Last", "nonexistinguser@email.com", null);
        string password = "valid-password";
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null!);
        _mockRepositoryManager.Setup(x => x.UserRepository.ValidateRegistrationPasswordAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _mockRepositoryManager.Setup(x => x.UserRepository.RegisterAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        var newGuid = await _authService.RegisterUserAsync(newUser, password);

        // Assert
        _mockRepositoryManager.Verify(x => x.UserRepository.RegisterAsync(It.IsAny<User>(), password), Times.Once);
        Assert.NotEqual(Guid.Empty, newGuid);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("testemail.com")]
    [InlineData("test@emailcom")]
    public async Task LoginUserAsync_EmailIsInvalid_ThrowsInvalidEmailException(string email)
    {
        // Arrange
        User user = new("First", "Last", email, null);
        string password = "password";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidEmailException>(() => _authService.LoginUserAsync(user, password));
    }

    [Fact]
    public async Task LoginUserAsync_EmailDoesNotExist_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        User user = new("First", "Last", "nonexistingemail@example.com", null);
        string password = "password";
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null!);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginUserAsync(user, password));
    }

    [Fact]
    public async Task LoginUserAsync_EmailDoesExistButPasswordIsIncorrect_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        User user = new("First", "Last", "existingemail@example.com", null);
        string password = "incorrect-password";
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new User());
        _mockRepositoryManager.Setup(x => x.UserRepository.ValidateLoginPasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginUserAsync(user, password));
    }

    [Fact]
    public async Task LoginUserAsync_EmailAndPasswordAreBothCorrect_ReturnsTokenObject()
    {
        // Arrange
        User user = new("First", "Last", "existingemail@example.com", null);
        string password = "password";
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new User());
        _mockRepositoryManager.Setup(x => x.UserRepository.ValidateLoginPasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        _mockRepositoryManager.Setup(x => x.TokenRepository.GenerateJwtAsync(It.IsAny<User>(), null))
            .ReturnsAsync(new Token("fake-token", "fake-refresh-token", DateTime.UtcNow));

        // Act
        var result = await _authService.LoginUserAsync(user, password);

        // Assert
        _mockRepositoryManager.Verify(x => x.TokenRepository.GenerateJwtAsync(It.IsAny<User>(), null), Times.Once);
        Assert.IsType<Token>(result);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Value);
        Assert.NotEmpty(result.RefreshToken);
    }

    [Fact]
    public async Task RefreshTokenAsync_TokenDoesNotExist_ThrowsInvalidRefreshTokenException()
    {
        // Arrange
        Token oldToken = new();
        _mockRepositoryManager.Setup(x => x.RefreshTokenRepository.GetByOldRefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync((RefreshToken)null!);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidRefreshTokenException>(() => _authService.RefreshTokenAsync(oldToken));
    }

    [Fact]
    public async Task RefreshTokenAsync_UserDoesNotExist_ThrowsInvalidRefreshTokenException()
    {
        // Arrange
        Token oldToken = new();
        _mockRepositoryManager.Setup(x => x.RefreshTokenRepository.GetByOldRefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new RefreshToken());
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null!);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidRefreshTokenException>(() => _authService.RefreshTokenAsync(oldToken));
    }

    [Fact]
    public async Task RefreshTokenAsync_ExistingTokenIsInvalid_ThrowsInvalidRefreshTokenException()
    {
        // Arrange
        Token oldToken = new();
        _mockRepositoryManager.Setup(x => x.RefreshTokenRepository.GetByOldRefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new RefreshToken());
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new User());
        _mockRepositoryManager.Setup(x => x.TokenRepository.RefreshJwtAsync(It.IsAny<Token>(), It.IsAny<User>(), It.IsAny<RefreshToken>()))
            .ThrowsAsync(new Exception(""));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidRefreshTokenException>(() => _authService.RefreshTokenAsync(oldToken));
    }

    [Fact]
    public async Task RefreshTokenAsync_ExistingTokenIsValid_ReturnsNewToken()
    {
        // Arrange
        Token oldToken = new("old-token", "old-refresh-token", DateTime.UtcNow);
        _mockRepositoryManager.Setup(x => x.RefreshTokenRepository.GetByOldRefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new RefreshToken());
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new User());
        _mockRepositoryManager.Setup(x => x.TokenRepository.RefreshJwtAsync(It.IsAny<Token>(), It.IsAny<User>(), It.IsAny<RefreshToken>()))
            .ReturnsAsync(new Token("new-token", "new-refresh-token", DateTime.UtcNow));

        // Act
        var result = await _authService.RefreshTokenAsync(oldToken);

        // Assert
        _mockRepositoryManager.Verify(x => x.TokenRepository.RefreshJwtAsync(It.IsAny<Token>(), It.IsAny<User>(), It.IsAny<RefreshToken>()), Times.Once);
        Assert.IsType<Token>(result);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Value);
        Assert.NotEmpty(result.RefreshToken);
    }
}