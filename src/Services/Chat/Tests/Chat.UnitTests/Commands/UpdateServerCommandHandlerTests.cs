using Chat.Application.Commands;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Aggregates.UserAggregate;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Commands;

public class UpdateServerCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly Mock<IUserManager> _mockUserManager;
    private readonly UpdateServerCommandHandler _updateServerCommandHandler;

    public UpdateServerCommandHandlerTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        Mock<IUserRepository> mockUserRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockUserManager = new Mock<IUserManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _mockRepositoryManager.Setup(x => x.UserRepository).Returns(mockUserRepository.Object);
        _updateServerCommandHandler = new(_mockRepositoryManager.Object, _mockUserManager.Object);
    }

    [Fact]
    public async Task Handle_ServerUpdated_ReturnsTrue()
    {
        // Arrange
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "");
        UpdateServerCommand command = new(targetServer, "Updated Name", "Updated Short Descrtion", "Updated Long Description", "user@example.com", "");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
            new Server(command.Name, command.ShortDescription, command.LongDescription, command.EditorEmail, ""));

        // Act
        var result = await _updateServerCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        _mockRepositoryManager.Verify(x => x.ServerRepository.UpdateAsync(It.IsAny<Server>()), Times.Once);
        Assert.True(result);
    }
}
