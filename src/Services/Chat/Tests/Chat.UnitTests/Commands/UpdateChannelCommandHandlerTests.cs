using Chat.Application.Commands;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Aggregates.UserAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Commands;

public class UpdateChannelCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly UpdateChannelCommandHandler _updateChannelCommandHandler;

    public UpdateChannelCommandHandlerTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        Mock<IUserRepository> mockUserRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _mockRepositoryManager.Setup(x => x.UserRepository).Returns(mockUserRepository.Object);
        _updateChannelCommandHandler = new(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task Handle_TargetServerIdIsInvalid_ThrowsServerNotFoundException()
    {
        // Arrange
        Guid targetServerId = Guid.NewGuid();
        Guid targetChannelId = Guid.NewGuid();
        UpdateChannelCommand command = new(targetServerId, targetChannelId, "Updated Name", "user@example.com");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Server)null!);

        // Act & Assert
        _mockRepositoryManager.Verify(x => x.ServerRepository.UpdateAsync(It.IsAny<Server>()), Times.Never);
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _updateChannelCommandHandler.Handle(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Handle_ChannelUpdated_ReturnsTrue()
    {
        // Arrange
        Guid targetServerId = Guid.NewGuid();
        Guid targetChannelId = Guid.NewGuid();
        UpdateChannelCommand command = new(targetServerId, targetChannelId, "Updated Name", "user@example.com");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Server("Target Server", "Short Desc", "Long Desc", ""));

        // Act
        var result = await _updateChannelCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        _mockRepositoryManager.Verify(x => x.ServerRepository.UpdateAsync(It.IsAny<Server>()), Times.Once);
        Assert.True(result);
    }
}
