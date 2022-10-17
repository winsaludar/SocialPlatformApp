using Chat.API.Controllers;
using Chat.API.Models;
using Chat.Application.Commands;
using Chat.Application.DTOs;
using Chat.Application.Queries;
using Chat.Application.Validators;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Chat.UnitTests.Controllers;

public class ChannelsControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly InlineValidator<GetChannelsQuery> _getChannelsQueryValidator;
    private readonly InlineValidator<CreateChannelCommand> _createChannelCommandValidator;
    private readonly InlineValidator<UpdateChannelCommand> _updateChannelCommandValidator;
    private readonly InlineValidator<DeleteChannelCommand> _deleteChannelCommandValidator;
    private ChannelsController _controller;

    public ChannelsControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _getChannelsQueryValidator = new InlineValidator<GetChannelsQuery>();
        _createChannelCommandValidator = new InlineValidator<CreateChannelCommand>();
        _updateChannelCommandValidator = new InlineValidator<UpdateChannelCommand>();
        _deleteChannelCommandValidator = new InlineValidator<DeleteChannelCommand>();

        Mock<IValidatorManager> mockValidatorManager = new();
        mockValidatorManager.Setup(x => x.GetChannelsQueryValidator).Returns(_getChannelsQueryValidator);
        mockValidatorManager.Setup(x => x.CreateChannelCommandValidator).Returns(_createChannelCommandValidator);
        mockValidatorManager.Setup(x => x.UpdateChannelCommandValidator).Returns(_updateChannelCommandValidator);
        mockValidatorManager.Setup(x => x.DeleteChannelCommandValidator).Returns(_deleteChannelCommandValidator);

        _controller = new ChannelsController(_mockMediator.Object, mockValidatorManager.Object);
    }

    [Fact]
    public async Task GetAllChannelsAsync_ValidationResultIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        _getChannelsQueryValidator.RuleFor(x => x.TargetServerId).Must(id => false);

        // Act
        var result = await _controller.GetAllChannelsAsync(Guid.Empty);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<GetChannelsQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<SerializableError>(badResult.Value);
        Assert.Equal("TargetServerId", errors.FirstOrDefault().Key);
    }

    [Fact]
    public async Task GetAllChannelsAsync_ResultIsEmpty_ReturnsOkObjectResultWithEmptyData()
    {
        // Arrange
        _getChannelsQueryValidator.RuleFor(x => x.TargetServerId).Must(id => true);
        _mockMediator.Setup(x => x.Send(It.IsAny<GetChannelsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Enumerable.Empty<ChannelDto>());

        // Act
        var result = await _controller.GetAllChannelsAsync(It.IsAny<Guid>());

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<GetChannelsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var channels = Assert.IsAssignableFrom<IEnumerable<ChannelDto>>(okResult.Value);
        Assert.Empty(channels);
    }

    [Fact]
    public async Task GetAllChannelsAsync_ResultIsNotEmpty_ReturnsOkObjectResultWithData()
    {
        // Arrange
        _getChannelsQueryValidator.RuleFor(x => x.TargetServerId).Must(id => true);
        _mockMediator.Setup(x => x.Send(It.IsAny<GetChannelsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(
            new List<ChannelDto>()
            {
                new ChannelDto(),
                new ChannelDto(),
                new ChannelDto()
            });

        // Act
        var result = await _controller.GetAllChannelsAsync(It.IsAny<Guid>());

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<GetChannelsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var channels = Assert.IsAssignableFrom<IEnumerable<ChannelDto>>(okResult.Value);
        Assert.NotEmpty(channels);
        Assert.Equal(3, channels.Count());
    }

    [Fact]
    public async Task CreateChannelAsync_UserIdentityIsNull_ReturnsUnauthorizedObjectResult()
    {
        // Arrange
        SetUpNullUserIdentity();
        Guid serverId = Guid.NewGuid();
        CreateUpdateChannelModel request = new() { Name = "Test Channel" };

        // Act
        var result = await _controller.CreateChannelAsync(serverId, request);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<CreateChannelCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task CreateChannelAsync_TargetServerNotFound_ReturnsServerNotFoundException()
    {
        // Arrange
        SetUpFakeUserIdentity();
        Guid serverId = Guid.NewGuid();
        CreateUpdateChannelModel request = new() { Name = "" };
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Server)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _controller.CreateChannelAsync(serverId, request));
    }

    [Fact]
    public async Task CreateChannelAsync_ValidationResultIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        Guid serverId = Guid.NewGuid();
        CreateUpdateChannelModel request = new() { Name = "" };
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Server("Target Server", "Short Desc", "Long Desc", "creator@example.com", ""));
        _createChannelCommandValidator.RuleFor(x => x.Name).Must(name => false);

        // Act
        var result = await _controller.CreateChannelAsync(serverId, request);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<CreateChannelCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<SerializableError>(badResult.Value);
        Assert.Equal("Name", errors.FirstOrDefault().Key);
    }

    [Fact]
    public async Task CreateChannelAsync_ValidationResultIsValid_ReturnsOkObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        Guid serverId = Guid.NewGuid();
        CreateUpdateChannelModel request = new() { Name = "Test Channel" };
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Server("Target Server", "Short Desc", "Long Desc", "creator@example.com", ""));
        _createChannelCommandValidator.RuleFor(x => x.Name).Must(name => true);
        _mockMediator.Setup(x => x.Send(It.IsAny<CreateChannelCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<Guid>());

        // Act
        var result = await _controller.CreateChannelAsync(serverId, request);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<CreateChannelCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateChannelAsync_UserIdentityIsNull_ReturnsUnauthorizedObjectResult()
    {
        // Arrange
        SetUpNullUserIdentity();
        Guid serverId = Guid.NewGuid();
        Guid channelId = Guid.NewGuid();
        CreateUpdateChannelModel model = new() { Name = "Updated Channel Name" };

        // Act
        var result = await _controller.UpdateChannelAsync(serverId, channelId, model);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<UpdateChannelCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task UpdateChannelAsync_TargetServerNotFound_ReturnsServerNotFoundException()
    {
        // Arrange
        SetUpFakeUserIdentity();
        Guid serverId = Guid.NewGuid();
        Guid channelId = Guid.NewGuid();
        CreateUpdateChannelModel model = new() { Name = "Updated Channel Name" };
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Server)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _controller.UpdateChannelAsync(serverId, channelId, model));
    }

    [Fact]
    public async Task UpdateChannelAsync_ValidationResultIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        Guid serverId = Guid.NewGuid();
        Guid channelId = Guid.NewGuid();
        CreateUpdateChannelModel model = new() { Name = "Updated Channel Name" };
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Server("Target Server", "Short Desc", "Long Desc", "creator@example.com", ""));
        _updateChannelCommandValidator.RuleFor(x => x.Name).Must(name => false);

        // Act
        var result = await _controller.UpdateChannelAsync(serverId, channelId, model);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<UpdateChannelCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<SerializableError>(badResult.Value);
        Assert.Equal("Name", errors.FirstOrDefault().Key);
    }

    [Fact]
    public async Task UpdateChannelAsync_ValidationResultIsValid_ReturnsOkObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        Guid serverId = Guid.NewGuid();
        Guid channelId = Guid.NewGuid();
        CreateUpdateChannelModel model = new() { Name = "Updated Channel Name" };
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Server("Target Server", "Short Desc", "Long Desc", "creator@example.com", ""));
        _updateChannelCommandValidator.RuleFor(x => x.Name).Must(name => true);
        _mockMediator.Setup(x => x.Send(It.IsAny<UpdateChannelCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<bool>());

        // Act
        var result = await _controller.UpdateChannelAsync(serverId, channelId, model);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<UpdateChannelCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task DeleteChannelAsync_TargetServerNotFound_ReturnsServerNotFoundException()
    {
        // Arrange
        SetUpFakeUserIdentity();
        Guid serverId = Guid.NewGuid();
        Guid channelId = Guid.NewGuid();
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Server)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _controller.DeleteChannelAsync(serverId, channelId));
    }

    [Fact]
    public async Task DeleteChannelAsync_ValidationResultIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        Guid serverId = Guid.NewGuid();
        Guid channelId = Guid.NewGuid();
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Server("Target Server", "Short Desc", "Long Desc", "creator@example.com", ""));
        _deleteChannelCommandValidator.RuleFor(x => x.TargetChannelId).Must(name => false);

        // Act
        var result = await _controller.DeleteChannelAsync(serverId, channelId);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<DeleteChannelCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<SerializableError>(badResult.Value);
        Assert.Equal("TargetChannelId", errors.FirstOrDefault().Key);
    }

    [Fact]
    public async Task DeleteChannelAsync_ValidationResultIsValid_ReturnsOkObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        Guid serverId = Guid.NewGuid();
        Guid channelId = Guid.NewGuid();
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Server("Target Server", "Short Desc", "Long Desc", "creator@example.com", ""));
        _deleteChannelCommandValidator.RuleFor(x => x.TargetChannelId).Must(name => true);
        _mockMediator.Setup(x => x.Send(It.IsAny<DeleteChannelCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<bool>());

        // Act
        var result = await _controller.DeleteChannelAsync(serverId, channelId);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<DeleteChannelCommand>(), It.IsAny<CancellationToken>()), Times.Once);
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
}
