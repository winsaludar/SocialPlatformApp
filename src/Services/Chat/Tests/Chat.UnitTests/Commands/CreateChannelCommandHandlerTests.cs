using Chat.Application.Commands;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Aggregates.UserAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Commands;

public class CreateChannelCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly CreateChannelCommandHandler _createChannelCommandHandler;

    public CreateChannelCommandHandlerTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        Mock<IUserRepository> mockUserRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _mockRepositoryManager.Setup(x => x.UserRepository).Returns(mockUserRepository.Object);
        _createChannelCommandHandler = new(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task Handle_TargetServerIdIsInvalid_ThrowsServerNotFoundException()
    {
        // Arrange
        Guid targetServerId = Guid.NewGuid();
        CreateChannelCommand command = new(targetServerId, "Test Channel");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Server)null!);

        // Act & Assert
        _mockRepositoryManager.Verify(x => x.ServerRepository.UpdateAsync(It.IsAny<Server>()), Times.Never);
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _createChannelCommandHandler.Handle(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Handle_ChannelCreated_ReturnsChannelId()
    {
        // Arrange
        Guid targetServerId = Guid.NewGuid();
        CreateChannelCommand command = new(targetServerId, "Test Channel");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
            new Server("Target Server", "Short Desc", "Long Desc", ""));

        // Act
        var result = await _createChannelCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        _mockRepositoryManager.Verify(x => x.ServerRepository.UpdateAsync(It.IsAny<Server>()), Times.Once);
        Assert.NotEqual(Guid.Empty, result);
    }
}
