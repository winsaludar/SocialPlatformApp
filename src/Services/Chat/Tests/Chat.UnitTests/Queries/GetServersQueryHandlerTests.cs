using Chat.Application.DTOs;
using Chat.Application.Queries;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Queries;

public class GetServersQueryHandlerTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly GetServersQueryHandler _getServersQueryHandler;

    public GetServersQueryHandlerTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _getServersQueryHandler = new(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsEmptyResult_ReturnsEmptyData()
    {
        // Arrange
        GetServersQuery query = new(1, 10, "");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(Enumerable.Empty<Server>());

        // Act
        var result = await _getServersQueryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        var servers = Assert.IsAssignableFrom<IEnumerable<ServerDto>>(result);
        Assert.Empty(servers);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsNonEmptyResult_ReturnsNonEmptyData()
    {
        // Arrange
        GetServersQuery query = new(1, 10, "");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(new List<Server>()
            {
                new Server("Server 1", "Short Description 1", "Short Description 1", "Thumbnail 1"),
                new Server("Server 2", "Short Description 2", "Short Description 2", "Thumbnail 2"),
                new Server("Server 3", "Short Description 3", "Short Description 3", "Thumbnail 3"),
            });

        // Act
        var result = await _getServersQueryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        var servers = Assert.IsAssignableFrom<IEnumerable<ServerDto>>(result);
        Assert.NotEmpty(servers);
        Assert.Equal(3, servers.Count());
    }
}
