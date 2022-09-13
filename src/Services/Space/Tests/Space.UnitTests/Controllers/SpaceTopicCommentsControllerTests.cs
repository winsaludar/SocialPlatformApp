using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Space.Contracts;
using Space.Presentation.Controllers;
using Space.Presentation.Models;
using Space.Services.Abstraction;
using System.Security.Claims;

namespace Space.UnitTests.Controllers;

public class SpaceTopicCommentsControllerTests
{
    private readonly Mock<IServiceManager> _mockService;
    private readonly SpaceTopicCommentsController _controller;

    public SpaceTopicCommentsControllerTests()
    {
        Mock<ISpaceService> mockSpaceService = new();
        Mock<ISoulService> mockSoulService = new();
        _mockService = new Mock<IServiceManager>();
        _mockService.Setup(x => x.SpaceService).Returns(mockSpaceService.Object);
        _mockService.Setup(x => x.SoulService).Returns(mockSoulService.Object);
        _controller = new SpaceTopicCommentsController(_mockService.Object);
    }

    [Fact]
    public async Task GetAllAsync_SpaceIdIsInvalid_ReturnsNotFoundObjectResult()
    {
        _mockService.Setup(x => x.SpaceService.GetByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync((SpaceDto)null!);

        var result = await _controller.GetAllAsync(It.IsAny<Guid>(), It.IsAny<Guid>());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetAllAsync_TopicIdIsInvalid_ReturnsNotFoundObjectResult()
    {
        _mockService.Setup(x => x.SpaceService.GetByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync(new SpaceDto());
        _mockService.Setup(x => x.SpaceService.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
           .ReturnsAsync((TopicDto)null!);

        var result = await _controller.GetAllAsync(It.IsAny<Guid>(), It.IsAny<Guid>());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetAllAsync_CommentsAreEmpty_ReturnsOkObjectResultWithEmptyData()
    {
        _mockService.Setup(x => x.SpaceService.GetByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync(new SpaceDto());
        _mockService.Setup(x => x.SpaceService.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
           .ReturnsAsync(new TopicDto());
        _mockService.Setup(x => x.SpaceService.GetAllCommentsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
           .ReturnsAsync((IEnumerable<CommentDto>)new List<CommentDto>());

        var result = await _controller.GetAllAsync(It.IsAny<Guid>(), It.IsAny<Guid>());

        var okResult = Assert.IsType<OkObjectResult>(result);
        var comments = Assert.IsType<List<CommentDto>>(okResult.Value);
        Assert.Empty(comments);
    }

    [Fact]
    public async Task GetAllAsync_CommentsAreNotEmpty_ReturnsOkObjectResultWithData()
    {
        _mockService.Setup(x => x.SpaceService.GetByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync(new SpaceDto());
        _mockService.Setup(x => x.SpaceService.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
           .ReturnsAsync(new TopicDto());
        _mockService.Setup(x => x.SpaceService.GetAllCommentsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
           .ReturnsAsync((IEnumerable<CommentDto>)new List<CommentDto>()
           {
               new CommentDto(),
               new CommentDto(),
               new CommentDto()
           });

        var result = await _controller.GetAllAsync(It.IsAny<Guid>(), It.IsAny<Guid>());

        var okResult = Assert.IsType<OkObjectResult>(result);
        var comments = Assert.IsType<List<CommentDto>>(okResult.Value);
        Assert.NotEmpty(comments);
        Assert.Equal(3, comments.Count());
    }

    [Fact]
    public async Task PostAsync_ModelStateIsInvalid_ReturnsBadRequestObjectResult()
    {
        Guid spaceId = Guid.NewGuid();
        Guid topicId = Guid.NewGuid();
        CreateEditCommentRequest request = new() { };
        _controller.ModelState.AddModelError("Comment", "Required");

        var result = await _controller.PostAsync(spaceId, topicId, request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PostAsync_UserIdentityIsNull_ReturnsUnauthorizedObjectResult()
    {
        // Setup a null User.Identity
        Mock<ClaimsPrincipal> user = new();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user.Object }
        };

        Guid spaceId = Guid.NewGuid();
        Guid topicId = Guid.NewGuid();
        CreateEditCommentRequest request = new() { Content = "Sample comment" };

        var result = await _controller.PostAsync(spaceId, topicId, request);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task PostAsync_RequestIsValid_ReturnsOkObjectResult()
    {
        CommentDto? createdTopic = null;
        _mockService.Setup(x => x.SpaceService.CreateCommentAsync(It.IsAny<CommentDto>()))
            .Callback<CommentDto>(x => createdTopic = x);

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
        Guid topicId = Guid.NewGuid();
        CreateEditCommentRequest request = new() { Content = "Sample comment" };

        var result = await _controller.PostAsync(spaceId, topicId, request);

        _mockService.Verify(x => x.SpaceService.CreateCommentAsync(It.IsAny<CommentDto>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(request.Content, createdTopic?.Content);
    }
}
