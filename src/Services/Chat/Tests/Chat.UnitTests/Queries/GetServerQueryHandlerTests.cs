using Chat.Application.Queries;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Queries;

public class GetServerQueryHandlerTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly GetServerQueryHandler _getServerQueryHandler;

    public GetServerQueryHandlerTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _getServerQueryHandler = new(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsNull_ReturnsNull()
    {
        // Arrange
        GetServerQuery query = new(Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Server)null!);

        // Act
        var result = await _getServerQueryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsServer_ReturnsServer()
    {
        // Arrange
        Server server = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
        server.SetId(Guid.NewGuid());
        GetServerQuery query = new(server.Id);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(server);

        // Act
        var result = await _getServerQueryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Server>(result);
    }
}
