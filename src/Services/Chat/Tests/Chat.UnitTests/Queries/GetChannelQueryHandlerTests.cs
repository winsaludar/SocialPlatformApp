using Chat.Application.Queries;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Queries;

public class GetChannelQueryHandlerTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly GetChannelQueryHandler _getChannelQueryHandler;

    public GetChannelQueryHandlerTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _getChannelQueryHandler = new(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsNull_ReturnsNull()
    {
        // Arrange
        GetChannelQuery query = new(Guid.NewGuid(), Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Server)null!);

        // Act
        var result = await _getChannelQueryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsServerWithNonExistingChannel_ReturnsNull()
    {
        // Arrange
        Server server = GetTargetServer();
        GetChannelQuery query = new(server.Id, Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(server);

        // Act
        var result = await _getChannelQueryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsServerWithExistingChannel_ReturnsChannel()
    {
        // Arrange
        Guid channelId = Guid.NewGuid();
        Server server = GetTargetServer();
        server.AddChannel(channelId, "channel", Guid.NewGuid(), DateTime.UtcNow);
        GetChannelQuery query = new(Guid.NewGuid(), channelId);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(server);

        // Act
        var result = await _getChannelQueryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Channel>(result);
    }

    private static Server GetTargetServer()
    {
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
        targetServer.SetId(Guid.NewGuid());

        return targetServer;
    }
}
