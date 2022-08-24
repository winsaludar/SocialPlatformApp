using Authentication.Domain.Entities;
using Authentication.Domain.Exceptions;
using Authentication.Domain.Repositories;
using Moq;

namespace Authentication.UnitTests.Entities;

public class UserTests
{
    private readonly Mock<IRepositoryManager> _mockRepo;

    public UserTests()
    {
        Mock<IUserRepository> mockUserRepo = new();
        Mock<ITokenRepository> mockTokenRepo = new();
        _mockRepo = new Mock<IRepositoryManager>();
        _mockRepo.Setup(x => x.UserRepository).Returns(mockUserRepo.Object);
        _mockRepo.Setup(x => x.TokenRepository).Returns(mockTokenRepo.Object);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("testemail.com")]
    [InlineData("test@emailcom")]
    public async Task RegisterAsync_EmailIsInvalid_ThrowsInvalidEmailException(string email)
    {
        User newUser = new()
        {
            FirstName = "First",
            LastName = "Last",
            Email = email
        };
        string password = "password";

        await Assert.ThrowsAsync<InvalidEmailException>(() => newUser.RegisterAsync(password, _mockRepo.Object));
    }

    [Fact]
    public async Task RegisterAsync_EmailAlreadyExist_ThrowsUserAlreadyExistException()
    {
        string email = "existinguser@email.com";
        User newUser = new()
        {
            FirstName = "First",
            LastName = "Last",
            Email = email
        };
        string password = "password";

        _mockRepo.Setup(x => x.UserRepository.GetByEmailAsync(email))
            .ReturnsAsync(new User { Email = email });

        await Assert.ThrowsAsync<UserAlreadyExistException>(() => newUser.RegisterAsync(password, _mockRepo.Object));
    }

    [Fact]
    public async Task RegisterAsync_PasswordIsInvalid_ThrowsInvalidPasswordException()
    {
        User newUser = new()
        {
            FirstName = "First",
            LastName = "Last",
            Email = "test@example.com"
        };
        string password = "";

        _mockRepo.Setup(x => x.UserRepository.ValidateRegistrationPasswordAsync(password))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<InvalidPasswordException>(() => newUser.RegisterAsync(password, _mockRepo.Object));
    }

    [Fact]
    public async Task RegisterAsync_EmailAndPasswordAreBothValid_CreateUser()
    {
        User? createdUser = null;
        string email = "nonexistinguser@email.com";
        string password = "P@$$w0rd1";
        User newUser = new()
        {
            FirstName = "First",
            LastName = "Last",
            Email = email,
        };

        _mockRepo.Setup(x => x.UserRepository.GetByEmailAsync(email))
            .ReturnsAsync((User)null!);
        _mockRepo.Setup(x => x.UserRepository.ValidateRegistrationPasswordAsync(password))
            .ReturnsAsync(true);
        _mockRepo.Setup(x => x.UserRepository.RegisterAsync(It.IsAny<User>(), password))
            .Callback<User, string>((x, y) => createdUser = x);

        await newUser.RegisterAsync(password, _mockRepo.Object);

        _mockRepo.Verify(x => x.UserRepository.RegisterAsync(It.IsAny<User>(), password), Times.Once);
        Assert.Equal(createdUser?.FirstName, newUser.FirstName);
        Assert.Equal(createdUser?.LastName, newUser.LastName);
        Assert.Equal(createdUser?.Email, newUser.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("testemail.com")]
    [InlineData("test@emailcom")]
    public async Task LoginAsync_EmailIsInvalid_ThrowsInvalidEmailException(string email)
    {
        User user = new() { Email = email };
        string password = "password";

        await Assert.ThrowsAsync<InvalidEmailException>(() => user.LoginAsync(password, _mockRepo.Object));
    }

    [Fact]
    public async Task LoginAsync_EmailDoesNotExist_ThrowsUnauthorizedAccessException()
    {
        string email = "nonexistingemail@example.com";
        string password = "password";
        User user = new() { Email = email };

        _mockRepo.Setup(x => x.UserRepository.GetByEmailAsync(email))
            .ReturnsAsync((User)null!);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => user.LoginAsync(password, _mockRepo.Object));
    }

    [Fact]
    public async Task LoginAsync_EmailDoesExistButPasswordIsIncorrect_ThrowsUnauthorizedAccessException()
    {
        string email = "existingemail@example.com";
        string password = "incorrect-password";
        User user = new() { Email = email };

        _mockRepo.Setup(x => x.UserRepository.GetByEmailAsync(email))
            .ReturnsAsync(new User());
        _mockRepo.Setup(x => x.UserRepository.ValidateLoginPasswordAsync(email, password))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => user.LoginAsync(password, _mockRepo.Object));
    }

    [Fact]
    public async Task LoginAsync_EmailAndPasswordAreBothCorrect_ReturnsTokenObject()
    {
        string email = "existingemail@example.com";
        string password = "correct-password";
        User user = new() { Email = email };

        _mockRepo.Setup(x => x.UserRepository.GetByEmailAsync(email))
            .ReturnsAsync(new User
            {
                Id = Guid.NewGuid(),
                FirstName = "First",
                LastName = "Last",
                Email = email
            });
        _mockRepo.Setup(x => x.UserRepository.ValidateLoginPasswordAsync(email, password))
            .ReturnsAsync(true);
        _mockRepo.Setup(x => x.TokenRepository.GenerateJwtAsync(It.IsAny<User>(), null))
            .ReturnsAsync(new Token { Value = "fake-token", RefreshToken = "fake-refresh-token" });

        var result = await user.LoginAsync(password, _mockRepo.Object);

        Assert.IsType<Token>(result);
        Assert.NotEmpty(result.Value);
    }
}
