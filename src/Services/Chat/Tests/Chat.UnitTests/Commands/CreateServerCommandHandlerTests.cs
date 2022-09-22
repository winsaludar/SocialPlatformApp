using Chat.Application.Commands;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Commands;

public class CreateServerCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly CreateServerCommandHandler _createServerCommandHandler;

    public CreateServerCommandHandlerTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _createServerCommandHandler = new(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task Handle_ServerCreated_ReturnsServerGuid()
    {
        // Arrange
        CreateServerCommand command = new("Server Name", "Short Description", "Long Description", "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.AddAsync(It.IsAny<Server>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _createServerCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEqual(Guid.Empty, result);
    }
}
