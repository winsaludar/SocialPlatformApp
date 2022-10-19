using Chat.Application.Commands;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Commands;

public class ChangeUsernameCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly ChangeUsernameCommandHandler _changeUsernameCommandHandler;

    public ChangeUsernameCommandHandlerTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _changeUsernameCommandHandler = new ChangeUsernameCommandHandler(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task Handle_UsernameChanged_ReturnsTrue()
    {
        // Arrange
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "");
        ChangeUsernameCommand command = new(targetServer, Guid.NewGuid(), "newUsername");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

        // Act
        var result = await _changeUsernameCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        _mockRepositoryManager.Verify(x => x.ServerRepository.UpdateAsync(It.IsAny<Server>()), Times.Once);
        Assert.True(result);
    }
}
