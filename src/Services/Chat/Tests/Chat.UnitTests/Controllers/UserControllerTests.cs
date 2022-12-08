using Chat.API.Controllers;
using Chat.API.Models;
using Chat.Application.Commands;
using Chat.Application.DTOs;
using Chat.Application.Queries;
using Chat.Application.Validators;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Aggregates.UserAggregate;
using Chat.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Chat.UnitTests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly InlineValidator<GetUserServersQuery> _getUserServersQueryValidator;
    private readonly InlineValidator<GetUserServerChannelsQuery> _getUserServerChannelsQueryValidator;
    private readonly InlineValidator<ChangeUsernameCommand> _changeUserCommandValidator;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _getUserServersQueryValidator = new InlineValidator<GetUserServersQuery>();
        _getUserServerChannelsQueryValidator = new InlineValidator<GetUserServerChannelsQuery>();
        _changeUserCommandValidator = new InlineValidator<ChangeUsernameCommand>();

        Mock<IValidatorManager> mockValidatorManager = new();
        mockValidatorManager.Setup(x => x.GetUserServersQueryValidator).Returns(_getUserServersQueryValidator);
        mockValidatorManager.Setup(x => x.GetUserServerChannelsQueryValidator).Returns(_getUserServerChannelsQueryValidator);
        mockValidatorManager.Setup(x => x.ChangeUsernameCommandValidator).Returns(_changeUserCommandValidator);

        _controller = new UserController(_mockMediator.Object, mockValidatorManager.Object);
    }

    [Fact]
    public async Task GetAllUserServersAsync_UserIdentityIsNull_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        SetUpNullUserIdentity();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.GetAllUserServersAsync());
    }

    [Fact]
    public async Task GetAllUserServersAsync_ValidationResultIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        _mockMediator.Setup(x => x.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetUser());
        _getUserServersQueryValidator.RuleFor(x => x.UserId).Must(userId => false);

        // Act
        var result = await _controller.GetAllUserServersAsync();

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<GetUserServersQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<SerializableError>(badResult.Value);
        Assert.Equal("UserId", errors.FirstOrDefault().Key);
    }

    [Fact]
    public async Task GetAllUserServersAsync_ResultIsEmpty_ReturnsOkObjectResultWithEmptyData()
    {
        // Arrange
        SetUpFakeUserIdentity();
        _mockMediator.Setup(x => x.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetUser());
        _mockMediator.Setup(x => x.Send(It.IsAny<GetUserServersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Enumerable.Empty<ServerDto>());

        // Act
        var result = await _controller.GetAllUserServersAsync();

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<GetUserServersQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var servers = Assert.IsAssignableFrom<IEnumerable<ServerDto>>(okResult.Value);
        Assert.Empty(servers);
    }

    [Fact]
    public async Task GetAllUserServersAsync_ResultIsNotEmpty_ReturnsOkObjectResultWithData()
    {
        // Arrange
        SetUpFakeUserIdentity();
        _mockMediator.Setup(x => x.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetUser());
        _mockMediator.Setup(x => x.Send(It.IsAny<GetUserServersQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(
            new List<ServerDto>()
            {
                new ServerDto(),
                new ServerDto(),
                new ServerDto()
            });

        // Act
        var result = await _controller.GetAllUserServersAsync();

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<GetUserServersQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var servers = Assert.IsAssignableFrom<IEnumerable<ServerDto>>(okResult.Value);
        Assert.NotEmpty(servers);
        Assert.Equal(3, servers.Count());
    }

    [Fact]
    public async Task GetAllUserServerChannelsAsync_UserIdentityIsNull_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        SetUpNullUserIdentity();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.GetAllUserServerChannelsAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAllUserServerChannelsAsync_ServerIdIsInvalid_ThrowsServerNotFoundException()
    {
        // Arrange
        SetUpFakeUserIdentity();
        _mockMediator.Setup(x => x.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetUser());
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Server)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _controller.GetAllUserServerChannelsAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAllUserServerChannelsAsync_ValidationResultIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        _mockMediator.Setup(x => x.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetUser());
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetTargetServer());
        _getUserServerChannelsQueryValidator.RuleFor(x => x.UserId).Must(userId => false);

        // Act
        var result = await _controller.GetAllUserServerChannelsAsync(Guid.NewGuid());

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<ChangeUsernameCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<SerializableError>(badResult.Value);
        Assert.Equal("UserId", errors.FirstOrDefault().Key);
    }

    [Fact]
    public async Task GetAllUserServerChannelsAsync_ResultIsEmpty_ReturnsOkObjectResultWithEmptyData()
    {
        // Arrange
        SetUpFakeUserIdentity();
        _mockMediator.Setup(x => x.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetUser());
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetTargetServer());
        _mockMediator.Setup(x => x.Send(It.IsAny<GetUserServerChannelsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Enumerable.Empty<ChannelDto>());

        // Act
        var result = await _controller.GetAllUserServerChannelsAsync(Guid.NewGuid());

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<GetUserServerChannelsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var channels = Assert.IsAssignableFrom<IEnumerable<ChannelDto>>(okResult.Value);
        Assert.Empty(channels);
    }

    [Fact]
    public async Task GetAllUserServerChannelsAsync_ResultIsNotEmpty_ReturnsOkObjectResultWithData()
    {
        // Arrange
        SetUpFakeUserIdentity();
        _mockMediator.Setup(x => x.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetUser());
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetTargetServer());
        _mockMediator.Setup(x => x.Send(It.IsAny<GetUserServerChannelsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<ChannelDto>()
        {
            new ChannelDto(),
            new ChannelDto(),
            new ChannelDto()
        });

        // Act
        var result = await _controller.GetAllUserServerChannelsAsync(Guid.NewGuid());

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<GetUserServerChannelsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var channels = Assert.IsAssignableFrom<IEnumerable<ChannelDto>>(okResult.Value);
        Assert.NotEmpty(channels);
        Assert.Equal(3, channels.Count());
    }

    [Fact]
    public async Task ChangeUsernameAsync_UserIdentityIsNull_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        SetUpNullUserIdentity();
        ChangeUsernameModel request = new() { ServerId = Guid.NewGuid(), NewUsername = "username" };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.ChangeUsernameAsync(request));
    }

    [Fact]
    public async Task ChangeUsernameAsync_ServerIdIsInvalid_ThrowsServerNotFoundException()
    {
        // Arrange
        SetUpFakeUserIdentity();
        ChangeUsernameModel request = new() { ServerId = Guid.NewGuid(), NewUsername = "username" };
        _mockMediator.Setup(x => x.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetUser());
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Server)null!);


        // Act & Assert
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _controller.ChangeUsernameAsync(request));
    }

    [Fact]
    public async Task ChangeUsernameAsync_ValidationResultIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        ChangeUsernameModel request = new() { ServerId = Guid.NewGuid(), NewUsername = "username" };
        _mockMediator.Setup(x => x.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetUser());
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetTargetServer());
        _changeUserCommandValidator.RuleFor(x => x.UserId).Must(userId => false);

        // Act
        var result = await _controller.ChangeUsernameAsync(request);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<ChangeUsernameCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<SerializableError>(badResult.Value);
        Assert.Equal("UserId", errors.FirstOrDefault().Key);
    }

    [Fact]
    public async Task ChangeUsernameAsync_ValidationResultIsValid_ReturnsOkRequestObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        ChangeUsernameModel request = new() { ServerId = Guid.NewGuid(), NewUsername = "username" };
        _mockMediator.Setup(x => x.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetUser());
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetTargetServer());
        _changeUserCommandValidator.RuleFor(x => x.UserId).Must(userId => true);
        _mockMediator.Setup(x => x.Send(It.IsAny<ChangeUsernameCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _controller.ChangeUsernameAsync(request);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<ChangeUsernameCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }

    private void SetUpNullUserIdentity()
    {
        // Setup a null User.Identity
        Mock<ClaimsPrincipal> user = new();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user.Object }
        };
    }

    private void SetUpFakeUserIdentity()
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
    }

    private static Server GetTargetServer()
    {
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
        targetServer.SetId(Guid.NewGuid());

        return targetServer;
    }

    private static User GetUser()
    {
        return new User(Guid.NewGuid(), "user", "user@example.com");
    }
}
