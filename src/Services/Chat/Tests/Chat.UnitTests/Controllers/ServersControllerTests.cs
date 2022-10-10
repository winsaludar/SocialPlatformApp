using Chat.API.Controllers;
using Chat.API.Models;
using Chat.Application.Commands;
using Chat.Application.DTOs;
using Chat.Application.Queries;
using Chat.Application.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using System.Security.Claims;

namespace Chat.UnitTests.Controllers;

public class ServersControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly InlineValidator<CreateServerCommand> _createServerCommandValidator;
    private readonly InlineValidator<GetServersQuery> _getServersQueryValidator;
    private readonly InlineValidator<UpdateServerCommand> _updateServerCommandValidator;
    private readonly InlineValidator<DeleteServerCommand> _deleteServerCommandValidator;
    private readonly ServersController _controller;

    public ServersControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _createServerCommandValidator = new InlineValidator<CreateServerCommand>();
        _getServersQueryValidator = new InlineValidator<GetServersQuery>();
        _updateServerCommandValidator = new InlineValidator<UpdateServerCommand>();
        _deleteServerCommandValidator = new InlineValidator<DeleteServerCommand>();

        Mock<IValidatorManager> mockValidatorManager = new();
        mockValidatorManager.Setup(x => x.CreateServerCommandValidator).Returns(_createServerCommandValidator);
        mockValidatorManager.Setup(x => x.GetServersQueryValidator).Returns(_getServersQueryValidator);
        mockValidatorManager.Setup(x => x.UpdateServerCommandValidator).Returns(_updateServerCommandValidator);
        mockValidatorManager.Setup(x => x.DeleteServerCommandValidator).Returns(_deleteServerCommandValidator);

        _controller = new ServersController(_mockMediator.Object, mockValidatorManager.Object);
    }

    [Fact]
    public async Task GetAllServersAsync_ValidationResultIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        GetServersQuery query = new(0, 10, "");
        _getServersQueryValidator.RuleFor(x => x.Page).Must(page => false);

        // Act
        var result = await _controller.GetAllServersAsync(0, 10, "");

        // Assert
        _mockMediator.Verify(x => x.Send(query, It.IsAny<CancellationToken>()), Times.Never);
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<SerializableError>(badResult.Value);
        Assert.Equal("Page", errors.FirstOrDefault().Key);
    }

    [Fact]
    public async Task GetAllServersAsync_ResultIsEmpty_ReturnsOkObjectResultWithEmptyData()
    {
        // Arrange
        GetServersQuery query = new(1, 10, "");
        _mockMediator.Setup(x => x.Send(query, It.IsAny<CancellationToken>())).ReturnsAsync(Enumerable.Empty<ServerDto>());

        // Act
        var result = await _controller.GetAllServersAsync(1, 10, "");

        // Assert
        _mockMediator.Verify(x => x.Send(query, It.IsAny<CancellationToken>()), Times.Once);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var servers = Assert.IsAssignableFrom<IEnumerable<ServerDto>>(okResult.Value);
        Assert.Empty(servers);
    }

    [Fact]
    public async Task GetAllServersAsync_ResultIsNotEmpty_ReturnsOkObjectResultWithData()
    {
        // Arrange
        GetServersQuery query = new(1, 10, "");
        _mockMediator.Setup(x => x.Send(query, It.IsAny<CancellationToken>())).ReturnsAsync(
            new List<ServerDto>()
            {
                new ServerDto(),
                new ServerDto(),
                new ServerDto()
            });

        // Act
        var result = await _controller.GetAllServersAsync(1, 10, "");

        // Assert
        _mockMediator.Verify(x => x.Send(query, It.IsAny<CancellationToken>()), Times.Once);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var servers = Assert.IsAssignableFrom<IEnumerable<ServerDto>>(okResult.Value);
        Assert.NotEmpty(servers);
        Assert.Equal(3, servers.Count());
    }

    [Fact]
    public async Task CreateServerAsync_UserIdentityIsNull_ReturnsUnauthorizedObjectResult()
    {
        // Arrange
        SetUpNullUserIdentity();
        CreateUpdateServerModel model = new()
        {
            Name = "Server Name",
            ShortDescription = "Short Description",
            LongDescription = "Long Description",
            Thumbnail = ""
        };

        // Act
        var result = await _controller.CreateServerAsync(model);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<CreateServerCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task CreateServerAsync_ValidationResultIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        CreateUpdateServerModel model = new()
        {
            Name = "Server Name",
            ShortDescription = "Short Description",
            LongDescription = "Long Description",
            Thumbnail = ""
        };
        _createServerCommandValidator.RuleFor(x => x.Name).Must(name => false);

        // Act
        var result = await _controller.CreateServerAsync(model);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<CreateServerCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<SerializableError>(badResult.Value);
        Assert.Equal("Name", errors.FirstOrDefault().Key);
    }

    [Fact]
    public async Task CreateServerAsync_ValidationResultIsValid_ReturnsOkObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        CreateUpdateServerModel model = new()
        {
            Name = "Server Name",
            ShortDescription = "Short Description",
            LongDescription = "Long Description",
            Thumbnail = ""
        };
        _createServerCommandValidator.RuleFor(x => x.Name).Must(name => true);
        _mockMediator.Setup(x => x.Send(It.IsAny<CreateServerCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<Guid>());

        // Act
        var result = await _controller.CreateServerAsync(model);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<CreateServerCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateServerAsync_UserIdentityIsNull_ReturnsUnauthorizedObjectResult()
    {
        // Arrange
        SetUpNullUserIdentity();
        Guid serverId = Guid.NewGuid();
        CreateUpdateServerModel model = new()
        {
            Name = "Server Name",
            ShortDescription = "Updated Short Description",
            LongDescription = "Updated Long Description",
            Thumbnail = ""
        };

        // Act
        var result = await _controller.UpdateServerAsync(serverId, model);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<UpdateServerCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task UpdateServerAsync_ValidationResultIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        Guid serverId = Guid.NewGuid();
        CreateUpdateServerModel model = new()
        {
            Name = "",
            ShortDescription = "Updated Short Description",
            LongDescription = "Updated Long Description",
            Thumbnail = ""
        };
        _updateServerCommandValidator.RuleFor(x => x.Name).Must(name => false);

        // Act
        var result = await _controller.UpdateServerAsync(serverId, model);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<UpdateServerCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<SerializableError>(badResult.Value);
        Assert.Equal("Name", errors.FirstOrDefault().Key);
    }

    [Fact]
    public async Task UpdateServerAsync_ValidationResultIsValid_ReturnsOkObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        Guid serverId = Guid.NewGuid();
        CreateUpdateServerModel model = new()
        {
            Name = "Updated Name",
            ShortDescription = "Updated Short Description",
            LongDescription = "Updated Long Description",
            Thumbnail = ""
        };
        _updateServerCommandValidator.RuleFor(x => x.Name).Must(name => true);
        _mockMediator.Setup(x => x.Send(It.IsAny<UpdateServerCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<bool>());

        // Act
        var result = await _controller.UpdateServerAsync(serverId, model);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<UpdateServerCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task DeleteServerAsync_UserIdentityIsNull_ReturnsUnauthorizedObjectResult()
    {
        // Arrange
        SetUpNullUserIdentity();
        Guid serverId = Guid.NewGuid();

        // Act
        var result = await _controller.DeleteServerAsync(serverId);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<UpdateServerCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task DeleteServerAsync_ValidationResultIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        Guid serverId = Guid.NewGuid();
        _deleteServerCommandValidator.RuleFor(x => x.TargetServerId).Must(name => false);

        // Act
        var result = await _controller.DeleteServerAsync(serverId);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<DeleteServerCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<SerializableError>(badResult.Value);
        Assert.Equal("TargetServerId", errors.FirstOrDefault().Key);
    }

    [Fact]
    public async Task DeleteServerAsync_ValidationResultIsValid_ReturnsOkObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        Guid serverId = Guid.NewGuid();
        _deleteServerCommandValidator.RuleFor(x => x.TargetServerId).Must(name => true);
        _mockMediator.Setup(x => x.Send(It.IsAny<DeleteServerCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<bool>());

        // Act
        var result = await _controller.DeleteServerAsync(serverId);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<DeleteServerCommand>(), It.IsAny<CancellationToken>()), Times.Once);
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
