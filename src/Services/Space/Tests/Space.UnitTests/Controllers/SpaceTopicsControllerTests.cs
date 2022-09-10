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
    public async Task GetAllAsync_SpaceIdIsInvalid_ReturnsNotFoundObjectResult()
    {
        _mockService.Setup(x => x.SpaceService.GetByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync((SpaceDto)null!);

        var result = await _controller.GetAllAsync(It.IsAny<Guid>());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetAllAsync_TopicsAreEmpty_ReturnsOkObjectResultWithEmptyData()
    {
        _mockService.Setup(x => x.SpaceService.GetByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync(new SpaceDto());
        _mockService.Setup(x => x.SpaceService.GetAllTopicsAsync(It.IsAny<Guid>()))
           .ReturnsAsync((IEnumerable<TopicDto>)new List<TopicDto>());

        var result = await _controller.GetAllAsync(It.IsAny<Guid>());

        var okResult = Assert.IsType<OkObjectResult>(result);
        var spaces = Assert.IsType<List<TopicDto>>(okResult.Value);
        Assert.Empty(spaces);
    }

    [Fact]
    public async Task GetAllAsync_TopicsAreNotEmpty_ReturnsOkObjectResultWithData()
    {
        _mockService.Setup(x => x.SpaceService.GetByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync(new SpaceDto());
        _mockService.Setup(x => x.SpaceService.GetAllTopicsAsync(It.IsAny<Guid>()))
           .ReturnsAsync((IEnumerable<TopicDto>)new List<TopicDto>
           {
               new TopicDto(),
               new TopicDto(),
               new TopicDto()
           });

        var result = await _controller.GetAllAsync(It.IsAny<Guid>());

        var okResult = Assert.IsType<OkObjectResult>(result);
        var topics = Assert.IsType<List<TopicDto>>(okResult.Value);
        Assert.NotEmpty(topics);
        Assert.Equal(3, topics.Count());
    }

    [Fact]
    public async Task GetBySlugAsync_OneOfTheSlugIsInvalid_ReturnsNotFoundObjectResult()
    {
        _mockService.Setup(x => x.SpaceService.GetTopicBySlugAsync(It.IsAny<string>(), It.IsAny<string>()))
           .ReturnsAsync((TopicDto)null!);

        var result = await _controller.GetBySlugAsync(It.IsAny<string>(), It.IsAny<string>());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetBySlugAsync_BothSlugsAreValid_ReturnsOkObjectResultWithData()
    {
        _mockService.Setup(x => x.SpaceService.GetTopicBySlugAsync(It.IsAny<string>(), It.IsAny<string>()))
           .ReturnsAsync(new TopicDto());

        var result = await _controller.GetBySlugAsync(It.IsAny<string>(), It.IsAny<string>());

        var okResult = Assert.IsType<OkObjectResult>(result);
        var topic = Assert.IsType<TopicDto>(okResult.Value);
        Assert.NotNull(topic);
    }

    [Fact]
    public async Task PostAsync_ModelStateIsInvalid_ReturnsBadRequestObjectResult()
    {
        Guid spaceId = Guid.NewGuid();
        CreateEditTopicRequest request = new() { };
        _controller.ModelState.AddModelError("Title", "Required");

        var result = await _controller.PostAsync(spaceId, request);

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
        CreateEditTopicRequest request = new()
        {
            Title = "Fake Title",
            Content = "Fake Content"
        };

        var result = await _controller.PostAsync(spaceId, request);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task PostAsync_RequestIsValid_ReturnsOkObjectResult()
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
        CreateEditTopicRequest request = new()
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

    [Fact]
    public async Task PutAsync_ModelStateIsInvalid_ReturnsBadRequestObjectResult()
    {
        Guid spaceId = Guid.NewGuid();
        Guid topicId = Guid.NewGuid();
        CreateEditTopicRequest request = new() { };
        _controller.ModelState.AddModelError("Title", "Required");

        var result = await _controller.PutAsync(spaceId, topicId, request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PutAsync_UserIdentityIsNull_ReturnsUnauthorizedObjectResult()
    {
        // Setup a null User.Identity
        Mock<ClaimsPrincipal> user = new();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user.Object }
        };

        Guid spaceId = Guid.NewGuid();
        Guid topicId = Guid.NewGuid();
        CreateEditTopicRequest request = new()
        {
            Title = "Updated Title",
            Content = "Updated Content"
        };

        var result = await _controller.PutAsync(spaceId, topicId, request);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task PutAsync_RequestIsValid_ReturnsOkObjectResult()
    {
        TopicDto? updatedTopic = null;
        _mockService.Setup(x => x.SpaceService.UpdateTopicAsync(It.IsAny<TopicDto>()))
            .Callback<TopicDto>(x => updatedTopic = x);

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
        CreateEditTopicRequest request = new()
        {
            Title = "Updated Title",
            Content = "Updated Content"
        };

        var result = await _controller.PutAsync(spaceId, topicId, request);

        _mockService.Verify(x => x.SpaceService.UpdateTopicAsync(It.IsAny<TopicDto>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(updatedTopic?.Title, request.Title);
        Assert.Equal(updatedTopic?.Content, request.Content);
    }

    [Fact]
    public async Task DeleteAsync_UserIdentityIsNull_ReturnsUnauthorizedObjectResult()
    {
        // Setup a null User.Identity
        Mock<ClaimsPrincipal> user = new();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user.Object }
        };

        Guid spaceId = Guid.NewGuid();
        Guid topicId = Guid.NewGuid();

        var result = await _controller.DeleteAsync(spaceId, topicId);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_RequestIsValid_ReturnsOkObjectResult()
    {
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

        var result = await _controller.DeleteAsync(spaceId, topicId);

        _mockService.Verify(x => x.SpaceService.DeleteTopicAsync(It.IsAny<TopicDto>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpvoteAsync_UserIdentityIsNull_ReturnsUnauthorizedObjectResult()
    {
        // Setup a null User.Identity
        Mock<ClaimsPrincipal> user = new();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user.Object }
        };

        Guid spaceId = Guid.NewGuid();
        Guid topicId = Guid.NewGuid();

        var result = await _controller.UpvoteAsync(spaceId, topicId);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task UpvoteAsync_RequestIsValid_ReturnsOkObjectResult()
    {
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

        var result = await _controller.UpvoteAsync(spaceId, topicId);

        _mockService.Verify(x => x.SpaceService.UpvoteTopicAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task DownvoteAsync_UserIdentityIsNull_ReturnsUnauthorizedObjectResult()
    {
        // Setup a null User.Identity
        Mock<ClaimsPrincipal> user = new();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user.Object }
        };

        Guid spaceId = Guid.NewGuid();
        Guid topicId = Guid.NewGuid();

        var result = await _controller.DownvoteAsync(spaceId, topicId);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task DownvoteAsync_RequestIsValid_ReturnsOkObjectResult()
    {
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

        var result = await _controller.DownvoteAsync(spaceId, topicId);

        _mockService.Verify(x => x.SpaceService.DownvoteTopicAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UnvoteAsync_UserIdentityIsNull_ReturnsUnauthorizedObjectResult()
    {
        // Setup a null User.Identity
        Mock<ClaimsPrincipal> user = new();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user.Object }
        };

        Guid spaceId = Guid.NewGuid();
        Guid topicId = Guid.NewGuid();

        var result = await _controller.UnvoteAsync(spaceId, topicId);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task UnvoteAsync_RequestIsValid_ReturnsOkObjectResult()
    {
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

        var result = await _controller.UnvoteAsync(spaceId, topicId);

        _mockService.Verify(x => x.SpaceService.UnvoteTopicAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }
}
