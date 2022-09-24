using Chat.Application.Commands;
using Chat.Application.Validators;
using Chat.Domain.Aggregates.UserAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Validators;

public class CreateUserCommandValidatorTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly CreateUserCommandValidator _validator;

    public CreateUserCommandValidatorTests()
    {
        Mock<IUserRepository> mockUserRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.UserRepository).Returns(mockUserRepository.Object);
        _validator = new CreateUserCommandValidator(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task AuthId_IsEmpty_ReturnsError()
    {
        // Arrange
        CreateUserCommand command = new(Guid.Empty, "Username", "user@example.com", Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "AuthId"));
    }

    [Fact]
    public async Task Username_IsEmpty_ReturnsError()
    {
        // Arrange
        CreateUserCommand command = new(Guid.NewGuid(), "", "user@example.com", Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "Username"));
    }

    [Fact]
    public async Task Username_AlreadyExist_ReturnsError()
    {
        // Arrange
        CreateUserCommand command = new(Guid.NewGuid(), "existing-username", "user@example.com", Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(new User(command.AuthId, command.Username, command.Email));

        // Act & Assert
        await Assert.ThrowsAsync<UsernameAlreadyExistException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Email_IsEmpty_ReturnsError()
    {
        // Arrange
        CreateUserCommand command = new(Guid.NewGuid(), "Username", "", Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "Email"));
    }

    [Fact]
    public async Task Email_AlreadyExist_ReturnsError()
    {
        // Arrange
        CreateUserCommand command = new(Guid.NewGuid(), "Username", "existing@example.com", Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new User(command.AuthId, command.Username, command.Email));

        // Act & Assert
        await Assert.ThrowsAsync<EmailAlreadyExistException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }
}
