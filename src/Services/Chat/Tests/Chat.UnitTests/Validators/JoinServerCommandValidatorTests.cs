using Chat.Application.Commands;
using Chat.Application.Validators;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Validators;

public class JoinServerCommandValidatorTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly JoinServerCommandValidator _validator;

    public JoinServerCommandValidatorTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _validator = new JoinServerCommandValidator(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task TargetServer_IsInvalid_ThrowsServerNotFoundException()
    {
        // Arrange
        Server targetServer = GetTargetServer();
        JoinServerCommand command = new(targetServer, Guid.NewGuid(), "user");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Server)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task UserId_IsEmpty_ReturnsAnError()
    {
        // Arrange
        Server targetServer = GetTargetServer();
        JoinServerCommand command = new(targetServer, Guid.Empty, "user");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "UserId"));
    }

    [Fact]
    public async Task Username_IsEmpty_ReturnsAnError()
    {
        // Arrange
        Server targetServer = GetTargetServer();
        JoinServerCommand command = new(targetServer, Guid.NewGuid(), "");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "Username"));
    }

    [Fact]
    public async Task User_IsAlreadyAMember_ThrowsUserIsAlreadyAMemberException()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        string username = "user";
        Server targetServer = GetTargetServer();
        targetServer.AddMember(userId, username, DateTime.UtcNow);
        JoinServerCommand command = new(targetServer, userId, username);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

        // Act & Assert
        await Assert.ThrowsAsync<UserIsAlreadyAMemberException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    private static Server GetTargetServer()
    {
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
        targetServer.SetId(Guid.NewGuid());

        return targetServer;
    }
}
