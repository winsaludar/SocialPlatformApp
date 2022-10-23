using Microsoft.AspNetCore.Mvc;
using Moq;
using Space.Contracts;
using Space.Presentation.Controllers;
using Space.Services.Abstraction;

namespace Space.UnitTests.Controllers;

public class SoulsControllerTests
{
    private readonly Mock<IServiceManager> _mockService;
    private readonly SoulsController _controller;

    public SoulsControllerTests()
    {
        Mock<ISpaceService> mockSpaceService = new();
        Mock<ISoulService> mockSoulService = new();
        _mockService = new Mock<IServiceManager>();
        _mockService.Setup(x => x.SpaceService).Returns(mockSpaceService.Object);
        _mockService.Setup(x => x.SoulService).Returns(mockSoulService.Object);
        _controller = new SoulsController(_mockService.Object);
    }

    [Fact]
    public async Task GetAllTopicsAsync_TopicsAreEmpty_ReturnsOkResultWithEmptyData()
    {
        Guid soulId = Guid.NewGuid();

        _mockService.Setup(x => x.SoulService.GetAllTopicsByIdAsync(soulId))
           .ReturnsAsync((IEnumerable<TopicDto>)new List<TopicDto>());

        var result = await _controller.GetAllTopicsAsync(soulId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var spaces = Assert.IsType<List<TopicDto>>(okResult.Value);
        Assert.Empty(spaces);
    }

    [Fact]
    public async Task GetAllTopicsAsync_TopicsAreNotEmpty_ReturnsOkResultWithData()
    {
        Guid soulId = Guid.NewGuid();

        _mockService.Setup(x => x.SoulService.GetAllTopicsByIdAsync(soulId))
           .ReturnsAsync((IEnumerable<TopicDto>)new List<TopicDto>
           {
               new TopicDto(),
               new TopicDto(),
               new TopicDto()
           });

        var result = await _controller.GetAllTopicsAsync(soulId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var topics = Assert.IsType<List<TopicDto>>(okResult.Value);
        Assert.NotEmpty(topics);
        Assert.Equal(3, topics.Count());
    }
}
