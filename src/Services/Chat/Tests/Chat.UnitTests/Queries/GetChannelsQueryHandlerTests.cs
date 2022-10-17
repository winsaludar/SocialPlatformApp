using Chat.Application.DTOs;
using Chat.Application.Queries;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Queries;

public class GetChannelsQueryHandlerTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly GetChannelsQueryHandler _getChannelsQueryHandler;

    public GetChannelsQueryHandlerTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _getChannelsQueryHandler = new(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsEmptyResult_ReturnsEmptyData()
    {
        // Arrange
        GetChannelsQuery query = new(Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(Enumerable.Empty<Server>());

        // Act
        var result = await _getChannelsQueryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        var channels = Assert.IsAssignableFrom<IEnumerable<ChannelDto>>(result);
        Assert.Empty(channels);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsAServerWithEmptyChannels_ReturnsEmptyData()
    {
        // Arrange
        GetChannelsQuery query = new(Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetTargetServer());

        // Act
        var result = await _getChannelsQueryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        var channels = Assert.IsAssignableFrom<IEnumerable<ChannelDto>>(result);
        Assert.Empty(channels);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsNonEmptyResult_ReturnsNonEmptyData()
    {
        // Arrange
        GetChannelsQuery query = new(Guid.NewGuid());
        Server server = GetTargetServer();
        server.AddChannel(Guid.NewGuid(), "Channel 1", Guid.NewGuid(), DateTime.UtcNow);
        server.AddChannel(Guid.NewGuid(), "Channel 2", Guid.NewGuid(), DateTime.UtcNow);
        server.AddChannel(Guid.NewGuid(), "Channel 3", Guid.NewGuid(), DateTime.UtcNow);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(server);

        // Act
        var result = await _getChannelsQueryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        var channels = Assert.IsAssignableFrom<IEnumerable<ChannelDto>>(result);
        Assert.NotEmpty(channels);
        Assert.Equal(3, channels.Count());
    }

    private static Server GetTargetServer()
    {
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
        targetServer.SetId(Guid.NewGuid());

        return targetServer;
    }
}
