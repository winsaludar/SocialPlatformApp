using Chat.Application.Commands;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Aggregates.UserAggregate;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Commands;

public class DeleteServerCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly DeleteServerCommandHandler _deleteServerCommandHandler;

    public DeleteServerCommandHandlerTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        Mock<IUserRepository> mockUserRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _mockRepositoryManager.Setup(x => x.UserRepository).Returns(mockUserRepository.Object);

        _deleteServerCommandHandler = new DeleteServerCommandHandler(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task Handle_ServerDeleted_ReturnsTrue()
    {
        // Arrange
        DeleteServerCommand command = new(Guid.NewGuid(), "user@example.com");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
            new Server("Server Name", "Short Description", "Long Description", command.DeleterEmail, ""));

        // Act
        var result = await _deleteServerCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        _mockRepositoryManager.Verify(x => x.ServerRepository.DeleteAsync(It.IsAny<Guid>()), Times.Once);
        Assert.True(result);
    }
}
