using Authentication.Contracts;
using Authentication.Domain.Entities;
using Authentication.Domain.Exceptions;
using Authentication.Domain.Repositories;
using Authentication.Services;
using Authentication.Services.Abstraction;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Authentication.UnitTests.Services;

public class ApplicationUserServiceTests
{
    private readonly Mock<IRepositoryManager> _mockRepo;
    private readonly IApplicationUserService _applicationUserService;

    public ApplicationUserServiceTests()
    {
        var mockApplicationUserRepo = new Mock<IApplicationUserRepository>();
        _mockRepo = new Mock<IRepositoryManager>();
        _mockRepo.SetupGet(x => x.ApplicationUserRepository).Returns(mockApplicationUserRepo.Object);

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x["JWT:ExpirationInMinutes"]).Returns("60");
        mockConfig.Setup(x => x["JWT:Secret"]).Returns("this-is-just-a-fake-key");
        mockConfig.Setup(x => x["JWT:Issuer"]).Returns("fake-issuer");
        mockConfig.Setup(x => x["JWT:Audience"]).Returns("fake-audience");

        _applicationUserService = new ApplicationUserService(_mockRepo.Object, mockConfig.Object);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("testemail.com")]
    [InlineData("test@emailcom")]
    public async Task GetByEmailAsync_InvalidEmail_ThrowsInvalidEmailException(string email) =>
        await Assert.ThrowsAsync<InvalidEmailException>(() => _applicationUserService.GetByEmailAsync(email));

    [Fact]
    public async Task GetByEmailAsync_EmailDoesNotExist_ThrowsUserNotFoundException()
    {
        string email = "notexistinguser@email.com";
        _mockRepo.Setup(x => x.ApplicationUserRepository.GetByEmailAsync(email))
            .ReturnsAsync((ApplicationUser)null!);

        await Assert.ThrowsAsync<UserNotFoundException>(() => _applicationUserService.GetByEmailAsync(email));
    }

    [Fact]
    public async Task GetByEmailAsync_EmailIsValidAndDoesExist_ReturnsApplicationUserDto()
    {
        string email = "existinguser@email.com";
        _mockRepo.Setup(x => x.ApplicationUserRepository.GetByEmailAsync(email))
            .ReturnsAsync(new ApplicationUser { Email = email });

        var result = await _applicationUserService.GetByEmailAsync(email);

        Assert.IsType<ApplicationUserDto>(result);
        Assert.Equal(email, result.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("testemail.com")]
    [InlineData("test@emailcom")]
    public async Task RegisterAsync_InvalidEmail_ThrowsInvalidEmailException(string email)
    {
        RegisterApplicationUserDto newUser = new()
        {
            FirstName = "First",
            LastName = "Last",
            Email = email,
            Password = "password"
        };

        await Assert.ThrowsAsync<InvalidEmailException>(() => _applicationUserService.RegisterAsync(newUser));
    }

    [Fact]
    public async Task RegisterAsync_EmailAlreadyExist_ThrowsUserAlreadyExistException()
    {
        string email = "existinguser@email.com";
        RegisterApplicationUserDto newUser = new()
        {
            FirstName = "First",
            LastName = "Last",
            Email = email,
            Password = "password"
        };
        _mockRepo.Setup(x => x.ApplicationUserRepository.GetByEmailAsync(email))
            .ReturnsAsync(new ApplicationUser { Email = email });

        await Assert.ThrowsAsync<UserAlreadyExistException>(() => _applicationUserService.RegisterAsync(newUser));
    }

    [Fact]
    public async Task RegisterAsync_InvalidPassword_ThrowsInvalidPasswordException()
    {
        RegisterApplicationUserDto newUser = new()
        {
            FirstName = "First",
            LastName = "Last",
            Email = "test@example.com",
            Password = ""
        };

        _mockRepo.Setup(x => x.ApplicationUserRepository.ValidateRegistrationPassword(newUser.Password))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<InvalidPasswordException>(() => _applicationUserService.RegisterAsync(newUser));
    }

    [Fact]
    public async Task RegisterAsync_EmailAndPasswordAreBothValid_CreateUser()
    {
        ApplicationUser? createdUser = null;
        string email = "nonexistinguser@email.com";
        string password = "P@$$w0rd1";
        _mockRepo.Setup(x => x.ApplicationUserRepository.GetByEmailAsync(email))
            .ReturnsAsync((ApplicationUser)null!);
        _mockRepo.Setup(x => x.ApplicationUserRepository.ValidateRegistrationPassword(password))
            .ReturnsAsync(true);
        _mockRepo.Setup(x => x.ApplicationUserRepository.RegisterAsync(It.IsAny<ApplicationUser>(), password))
            .Callback<ApplicationUser, string>((x, y) => createdUser = x);
        RegisterApplicationUserDto newUser = new()
        {
            FirstName = "First",
            LastName = "Last",
            Email = email,
            Password = password
        };

        await _applicationUserService.RegisterAsync(newUser);

        _mockRepo.Verify(x => x.ApplicationUserRepository.RegisterAsync(It.IsAny<ApplicationUser>(), password), Times.Once);
        Assert.Equal(createdUser?.FirstName, newUser.FirstName);
        Assert.Equal(createdUser?.LastName, newUser.LastName);
        Assert.Equal(createdUser?.Email, newUser.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("testemail.com")]
    [InlineData("test@emailcom")]
    public async Task LoginAsync_InvalidEmail_ThrowsInvalidEmailException(string email)
    {
        LoginUserDto user = new()
        {
            Email = email,
            Password = "password"
        };

        await Assert.ThrowsAsync<InvalidEmailException>(() => _applicationUserService.LoginAsync(user));
    }

    [Fact]
    public async Task LoginAsync_EmailDoesNotExist_ThrowsUnauthorizedAccessException()
    {
        string email = "nonexistingemail@example.com";
        LoginUserDto user = new() { Email = email, Password = "password" };
        _mockRepo.Setup(x => x.ApplicationUserRepository.GetByEmailAsync(email))
            .ReturnsAsync((ApplicationUser)null!);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _applicationUserService.LoginAsync(user));
    }

    [Fact]
    public async Task LoginAsync_EmailDoesExistButPasswordIsIncorrect_ThrowsUnauthorizedAccessException()
    {
        string email = "existingemail@example.com";
        string password = "incorrect-password";
        LoginUserDto user = new() { Email = email, Password = password };
        _mockRepo.Setup(x => x.ApplicationUserRepository.GetByEmailAsync(email))
            .ReturnsAsync(new ApplicationUser());
        _mockRepo.Setup(x => x.ApplicationUserRepository.ValidateLoginPassword(email, password))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _applicationUserService.LoginAsync(user));
    }

    [Fact]
    public async Task LoginAsync_EmailAndPasswordAreBothCorrect_ReturnsTokenObject()
    {
        string email = "existingemail@example.com";
        string password = "correct-password";
        LoginUserDto user = new() { Email = email, Password = password };
        _mockRepo.Setup(x => x.ApplicationUserRepository.GetByEmailAsync(email))
            .ReturnsAsync(new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = "First",
                LastName = "Last",
                Email = email
            });
        _mockRepo.Setup(x => x.ApplicationUserRepository.ValidateLoginPassword(email, password))
            .ReturnsAsync(true);

        var result = await _applicationUserService.LoginAsync(user);

        Assert.IsType<TokenDto>(result);
        Assert.NotEmpty(result.Token);
    }
}
