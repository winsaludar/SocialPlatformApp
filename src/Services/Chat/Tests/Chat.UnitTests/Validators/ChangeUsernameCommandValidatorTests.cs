using Chat.Application.Commands;
using Chat.Application.Validators;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Aggregates.UserAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Validators;

public class ChangeUsernameCommandValidatorTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly ChangeUsernameCommandValidator _validator;

    public ChangeUsernameCommandValidatorTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        Mock<IUserRepository> mockUserRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _mockRepositoryManager.Setup(x => x.UserRepository).Returns(mockUserRepository.Object);
        _validator = new ChangeUsernameCommandValidator(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task TargetServer_IsInvalid_ThrowsServerNotFoundException()
    {
        // Arrange
        Server targetServer = GetTargetServer();
        ChangeUsernameCommand command = new(targetServer, Guid.NewGuid(), "newUsername");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Server)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task UserId_IsEmpty_ReturnsAnError()
    {
        // Arrange
        Server targetServer = GetTargetServer();
        targetServer.AddMember(Guid.Empty, "notexistinguser", DateTime.UtcNow);
        ChangeUsernameCommand command = new(targetServer, Guid.Empty, "newUsername");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetUser());

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "UserId"));
    }

    [Fact]
    public async Task User_IsNotAMember_ThrowsUserIsNotAMemberException()
    {
        // Arrange
        User existingUser = GetUser();
        Server targetServer = GetTargetServer();
        ChangeUsernameCommand command = new(targetServer, existingUser.Id, "newUsername");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<UserIsNotAMemberException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    private static Server GetTargetServer()
    {
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
        targetServer.SetId(Guid.NewGuid());

        return targetServer;
    }

    private static User GetUser()
    {
        User user = new(Guid.NewGuid(), "user", "user@example.com");
        user.SetId(Guid.NewGuid());

        return user;
    }
}
