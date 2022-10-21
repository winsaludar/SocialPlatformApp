using Chat.API.Controllers;
using Chat.API.Models;
using Chat.Application.Commands;
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

public class ModeratorsControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly InlineValidator<AddModeratorCommand> _addModeratorCommandValidator;
    private readonly ModeratorsController _controller;

    public ModeratorsControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _addModeratorCommandValidator = new InlineValidator<AddModeratorCommand>();

        Mock<IValidatorManager> validatorManager = new();
        validatorManager.Setup(x => x.AddModeratorCommandValidator).Returns(_addModeratorCommandValidator);

        _controller = new ModeratorsController(_mockMediator.Object, validatorManager.Object);
    }

    [Fact]
    public async Task AddModeratorAsync_UserIdentityIsNull_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        SetUpNullUserIdentity();
        Guid serverId = Guid.NewGuid();
        AddRemoveModeratorModel request = new() { UserId = Guid.NewGuid() };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.AddModeratorAsync(serverId, request));
    }

    [Fact]
    public async Task AddModeratorAsync_ServerIdIsInvalid_ThrowsServerNotFoundException()
    {
        // Arrange
        SetUpFakeUserIdentity();
        Guid serverId = Guid.NewGuid();
        AddRemoveModeratorModel request = new() { UserId = Guid.NewGuid() };
        _mockMediator.Setup(x => x.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetUser());
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Server)null!);


        // Act & Assert
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _controller.AddModeratorAsync(serverId, request));
    }

    [Fact]
    public async Task AddModeratorAsync_ValidationResultIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        Guid serverId = Guid.NewGuid();
        AddRemoveModeratorModel request = new() { UserId = Guid.NewGuid() };
        _mockMediator.Setup(x => x.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetUser());
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetTargetServer());
        _addModeratorCommandValidator.RuleFor(x => x.UserId).Must(userId => false);

        // Act
        var result = await _controller.AddModeratorAsync(serverId, request);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<AddModeratorCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<SerializableError>(badResult.Value);
        Assert.Equal("UserId", errors.FirstOrDefault().Key);
    }

    [Fact]
    public async Task AddModeratorAsync_ValidationResultIsValid_ReturnsOkRequestObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        Guid serverId = Guid.NewGuid();
        AddRemoveModeratorModel request = new() { UserId = Guid.NewGuid() };
        _mockMediator.Setup(x => x.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetUser());
        _mockMediator.Setup(x => x.Send(It.IsAny<GetServerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetTargetServer());
        _addModeratorCommandValidator.RuleFor(x => x.UserId).Must(userId => true);
        _mockMediator.Setup(x => x.Send(It.IsAny<AddModeratorCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _controller.AddModeratorAsync(serverId, request);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<AddModeratorCommand>(), It.IsAny<CancellationToken>()), Times.Once);
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
