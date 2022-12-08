using Chat.Application.DTOs;
using Chat.Application.Queries;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Aggregates.UserAggregate;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Queries;

public class GetUserServersQueryHandlerTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly GetUserServersQueryHandler _getUserServersQueryHandler;

    public GetUserServersQueryHandlerTests()
    {
        Mock<IUserRepository> mockUserRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.UserRepository).Returns(mockUserRepository.Object);
        _getUserServersQueryHandler = new GetUserServersQueryHandler(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsEmptyResult_ReturnsEmptyData()
    {
        // Arrange
        GetUserServersQuery query = new(Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.UserRepository.GetUserServersAsync(It.IsAny<Guid>())).ReturnsAsync(Enumerable.Empty<Server>());

        // Act
        var result = await _getUserServersQueryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        var servers = Assert.IsAssignableFrom<IEnumerable<ServerDto>>(result);
        Assert.Empty(servers);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsNonEmptyResult_ReturnsNonEmptyData()
    {
        // Arrange
        GetUserServersQuery query = new(Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.UserRepository.GetUserServersAsync(It.IsAny<Guid>())).ReturnsAsync(
        new List<Server>()
            {
                new Server("Server 1", "Short Description 1", "Short Description 1", "Thumbnail 1"),
                new Server("Server 2", "Short Description 2", "Short Description 2", "Thumbnail 2"),
                new Server("Server 3", "Short Description 3", "Short Description 3", "Thumbnail 3"),
            });

        // Act
        var result = await _getUserServersQueryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        var servers = Assert.IsAssignableFrom<IEnumerable<ServerDto>>(result);
        Assert.NotEmpty(servers);
        Assert.Equal(3, servers.Count());
    }
}
