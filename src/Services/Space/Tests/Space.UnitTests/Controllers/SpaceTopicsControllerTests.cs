using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Space.Contracts;
using Space.Presentation.Controllers;
using Space.Presentation.Models;
using Space.Services.Abstraction;
using System.Security.Claims;

namespace Space.UnitTests.Controllers;

public class SpaceTopicsControllerTests
{
    private readonly Mock<IServiceManager> _mockService;
    private readonly SpaceTopicsController _controller;

    public SpaceTopicsControllerTests()
    {
        Mock<ISpaceService> mockSpaceService = new();
        Mock<ISoulService> mockSoulService = new();
        _mockService = new Mock<IServiceManager>();
        _mockService.Setup(x => x.SpaceService).Returns(mockSpaceService.Object);
        _mockService.Setup(x => x.SoulService).Returns(mockSoulService.Object);
        _controller = new SpaceTopicsController(_mockService.Object);
    }

    [Fact]
    public async Task GetAsync_TopicsAreEmpty_ReturnsOkResultWithEmptyData()
    {
        Guid spaceId = Guid.NewGuid();

        _mockService.Setup(x => x.SpaceService.GetAllTopicsAsync(spaceId))
           .ReturnsAsync((IEnumerable<TopicDto>)new List<TopicDto>());

        var result = await _controller.GetAsync(spaceId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var spaces = Assert.IsType<List<TopicDto>>(okResult.Value);
        Assert.Empty(spaces);
    }

    [Fact]
    public async Task GetAsync_TopicsAreNotEmpty_ReturnsOkResultWithData()
    {
        Guid spaceId = Guid.NewGuid();

        _mockService.Setup(x => x.SpaceService.GetAllTopicsAsync(spaceId))
           .ReturnsAsync((IEnumerable<TopicDto>)new List<TopicDto>
           {
               new TopicDto(),
               new TopicDto(),
               new TopicDto()
           });

        var result = await _controller.GetAsync(spaceId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var topics = Assert.IsType<List<TopicDto>>(okResult.Value);
        Assert.NotEmpty(topics);
        Assert.Equal(3, topics.Count());
    }

    [Fact]
    public async Task PostAsync_ModelStateIsInvalid_ReturnsBadRequest()
    {
        Guid spaceId = Guid.NewGuid();
        CreateSpaceTopicRequest request = new() { };
        _controller.ModelState.AddModelError("Title", "Required");

        var result = await _controller.PostAsync(spaceId, request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PostAsync_UserIdentityIsNull_ReturnsUnauthorized()
    {
        // Setup a null User.Identity
        Mock<ClaimsPrincipal> user = new();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user.Object }
        };

        Guid spaceId = Guid.NewGuid();
        CreateSpaceTopicRequest request = new()
        {
            Title = "Fake Title",
            Content = "Fake Content"
        };

        var result = await _controller.PostAsync(spaceId, request);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task PostAsync_RequestIsValid_ReturnsOkResponse()
    {
        TopicDto? createdTopic = null;
        _mockService.Setup(x => x.SpaceService.CreateTopicAsync(It.IsAny<TopicDto>()))
            .Callback<TopicDto>(x => createdTopic = x);

        // Setup User.Identity
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name, "test@example.com"),
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim("name", "test@example.com"),
        };
        ClaimsIdentity identity = new(claims, "Test");
        ClaimsPrincipal user = new(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        Guid spaceId = Guid.NewGuid();
        CreateSpaceTopicRequest request = new()
        {
            Title = "Fake Title",
            Content = "Fake Content"
        };

        var result = await _controller.PostAsync(spaceId, request);

        _mockService.Verify(x => x.SpaceService.CreateTopicAsync(It.IsAny<TopicDto>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(createdTopic?.Title, request.Title);
        Assert.Equal(createdTopic?.Content, request.Content);
    }
}
