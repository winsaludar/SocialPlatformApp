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
