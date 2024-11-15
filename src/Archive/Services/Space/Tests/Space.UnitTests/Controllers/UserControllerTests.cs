﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Space.Contracts;
using Space.Presentation.Controllers;
using Space.Services.Abstraction;
using System.Security.Claims;

namespace Space.UnitTests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IServiceManager> _mockService;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        Mock<ISpaceService> mockSpaceService = new();
        Mock<ISoulService> mockSoulService = new();
        _mockService = new Mock<IServiceManager>();
        _mockService.Setup(x => x.SpaceService).Returns(mockSpaceService.Object);
        _mockService.Setup(x => x.SoulService).Returns(mockSoulService.Object);
        _controller = new UserController(_mockService.Object);
    }

    [Fact]
    public async Task GetAllModeratedSpacesAsync_UserIdentityIsNull_ReturnsUnauthorizedObjectResult()
    {
        // Setup a null User.Identity
        Mock<ClaimsPrincipal> user = new();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user.Object }
        };

        var result = await _controller.GetAllModeratedSpacesAsync();

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task GetAllModeratedSpacesAsync_SpacesAreEmpty_ReturnsOkResultWithEmptyData()
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

        _mockService.Setup(x => x.SoulService.GetAllModeratedSpacesByEmailAsync(It.IsAny<string>()))
           .ReturnsAsync((IEnumerable<SpaceDto>)new List<SpaceDto>());

        var result = await _controller.GetAllModeratedSpacesAsync();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var spaces = Assert.IsType<List<SpaceDto>>(okResult.Value);
        Assert.Empty(spaces);
    }

    [Fact]
    public async Task GetAllModeratedSpacesAsync_SpacesAreNotEmpty_ReturnsOkResultWithData()
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

        _mockService.Setup(x => x.SoulService.GetAllModeratedSpacesByEmailAsync(It.IsAny<string>()))
           .ReturnsAsync((IEnumerable<SpaceDto>)new List<SpaceDto>
           {
               new SpaceDto(),
               new SpaceDto(),
               new SpaceDto()
           });

        var result = await _controller.GetAllModeratedSpacesAsync();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var spaces = Assert.IsType<List<SpaceDto>>(okResult.Value);
        Assert.NotEmpty(spaces);
        Assert.Equal(3, spaces.Count());
    }

    [Fact]
    public async Task GetAllTopicsAsync_UserIdentityIsNull_ReturnsUnauthorizedObjectResult()
    {
        // Setup a null User.Identity
        Mock<ClaimsPrincipal> user = new();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user.Object }
        };

        var result = await _controller.GetAllTopicsAsync();

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task GetAllTopicsAsync_TopicsAreEmpty_ReturnsOkResultWithEmptyData()
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

        _mockService.Setup(x => x.SoulService.GetAllTopicsByEmailAsync(It.IsAny<string>()))
           .ReturnsAsync((IEnumerable<TopicDto>)new List<TopicDto>());

        var result = await _controller.GetAllTopicsAsync();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var topics = Assert.IsType<List<TopicDto>>(okResult.Value);
        Assert.Empty(topics);
    }

    [Fact]
    public async Task GetAllTopicsAsync_TopicsAreNotEmpty_ReturnsOkResultWithData()
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

        _mockService.Setup(x => x.SoulService.GetAllTopicsByEmailAsync(It.IsAny<string>()))
           .ReturnsAsync((IEnumerable<TopicDto>)new List<TopicDto>
           {
               new TopicDto(),
               new TopicDto(),
               new TopicDto()
           });

        var result = await _controller.GetAllTopicsAsync();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var topics = Assert.IsType<List<TopicDto>>(okResult.Value);
        Assert.NotEmpty(topics);
        Assert.Equal(3, topics.Count());
    }

    [Fact]
    public async Task GetAllCommentsAsync_UserIdentityIsNull_ReturnsUnauthorizedObjectResult()
    {
        // Setup a null User.Identity
        Mock<ClaimsPrincipal> user = new();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user.Object }
        };

        var result = await _controller.GetAllCommentsAsync();

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task GetAllCommentsAsync_CommentsAreEmpty_ReturnsOkResultWithEmptyData()
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

        _mockService.Setup(x => x.SoulService.GetAllCommentsByEmailAsync(It.IsAny<string>()))
           .ReturnsAsync((IEnumerable<CommentDto>)new List<CommentDto>());

        var result = await _controller.GetAllCommentsAsync();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var comments = Assert.IsType<List<CommentDto>>(okResult.Value);
        Assert.Empty(comments);
    }

    [Fact]
    public async Task GetAllCommentsAsync_CommentsAreNotEmpty_ReturnsOkResultWithData()
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

        _mockService.Setup(x => x.SoulService.GetAllCommentsByEmailAsync(It.IsAny<string>()))
           .ReturnsAsync((IEnumerable<CommentDto>)new List<CommentDto>
           {
               new CommentDto(),
               new CommentDto(),
               new CommentDto()
           });

        var result = await _controller.GetAllCommentsAsync();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var comments = Assert.IsType<List<CommentDto>>(okResult.Value);
        Assert.NotEmpty(comments);
        Assert.Equal(3, comments.Count());
    }
}
