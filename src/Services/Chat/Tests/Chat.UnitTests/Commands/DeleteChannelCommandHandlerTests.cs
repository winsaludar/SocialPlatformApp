using Chat.Application.Commands;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Aggregates.UserAggregate;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Commands;

public class DeleteChannelCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly DeleteChannelCommandHandler _deleteChannelCommandHandler;

    public DeleteChannelCommandHandlerTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        Mock<IUserRepository> mockUserRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _mockRepositoryManager.Setup(x => x.UserRepository).Returns(mockUserRepository.Object);
        _deleteChannelCommandHandler = new(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task Handle_ChannelDeleted_ReturnsTrue()
    {
        // Arrange
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "");
        Guid targetChannelId = Guid.NewGuid();
        DeleteChannelCommand command = new(targetServer, targetChannelId, targetServer.CreatedById);

        // Act
        var result = await _deleteChannelCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        _mockRepositoryManager.Verify(x => x.ServerRepository.UpdateAsync(It.IsAny<Server>()), Times.Once);
        Assert.True(result);
    }
}
