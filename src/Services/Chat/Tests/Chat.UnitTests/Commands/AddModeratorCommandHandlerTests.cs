using Chat.Application.Commands;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Commands;

public class AddModeratorCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly AddModeratorCommandHandler _addModeratorCommandHandler;

    public AddModeratorCommandHandlerTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _addModeratorCommandHandler = new AddModeratorCommandHandler(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task Handle_ModeratorAdded_ReturnsTrue()
    {
        // Arrange
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "");
        AddModeratorCommand command = new(targetServer, Guid.NewGuid(), Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

        // Act
        var result = await _addModeratorCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        _mockRepositoryManager.Verify(x => x.ServerRepository.UpdateAsync(It.IsAny<Server>()), Times.Once);
        Assert.True(result);
    }
}
