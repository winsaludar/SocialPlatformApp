using Chat.Application.DTOs;
using Chat.Application.Queries;
using Chat.Domain.Aggregates.ServerAggregate;
using Moq;

namespace Chat.UnitTests.Queries;

public class GetUserServerChannelsQueryHandlerTests
{
    private readonly GetUserServerChannelsQueryHandler _getUserServerChannelsQueryHandler;

    public GetUserServerChannelsQueryHandlerTests()
    {
        _getUserServerChannelsQueryHandler = new GetUserServerChannelsQueryHandler();
    }

    [Fact]
    public async Task Handle_ServerReturnsEmptyResult_ReturnsEmptyData()
    {
        // Arrange
        Server server = new("Server", "Short Desc", "Long Desc", "creator@example.com", "");
        GetUserServerChannelsQuery query = new(Guid.NewGuid(), server);

        // Act
        var result = await _getUserServerChannelsQueryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        var channels = Assert.IsAssignableFrom<IEnumerable<ChannelDto>>(result);
        Assert.Empty(channels);
    }

    [Fact]
    public async Task Handle_ServerReturnsNonEmptyResult_ReturnsNonEmptyData()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Server server = new("Server", "Short Desc", "Long Desc", "creator@example.com", "");
        server.AddChannel(Guid.NewGuid(), "Ch1", false, userId, DateTime.UtcNow, null, null);
        server.AddChannel(Guid.NewGuid(), "Ch2", true, userId, DateTime.UtcNow, null, null);
        server.AddChannel(Guid.NewGuid(), "Ch3", true, Guid.NewGuid(), DateTime.UtcNow, null, null); // userId is not a member here
        GetUserServerChannelsQuery query = new(userId, server);

        // Act
        var result = await _getUserServerChannelsQueryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        var channels = Assert.IsAssignableFrom<IEnumerable<ChannelDto>>(result);
        Assert.NotEmpty(channels);
        Assert.Equal(2, channels.Count());
    }
}
