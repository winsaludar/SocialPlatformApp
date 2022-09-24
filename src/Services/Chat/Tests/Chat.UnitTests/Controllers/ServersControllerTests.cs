using Chat.API.Controllers;
using Chat.Application.Commands;
using Chat.Application.DTOs;
using Chat.Application.Queries;
using Chat.Application.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Chat.UnitTests.Controllers;

public class ServersControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly InlineValidator<CreateServerCommand> _createServerCommandValidator;
    private readonly InlineValidator<GetServersQuery> _getServersQueryValidator;
    private readonly ServersController _controller;

    public ServersControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _createServerCommandValidator = new InlineValidator<CreateServerCommand>();
        _getServersQueryValidator = new InlineValidator<GetServersQuery>();

        Mock<IValidatorManager> mockValidatorManager = new();
        mockValidatorManager.Setup(x => x.CreateServerCommandValidator).Returns(_createServerCommandValidator);
        mockValidatorManager.Setup(x => x.GetServersQueryValidator).Returns(_getServersQueryValidator);

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
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<SerializableError>(badResult.Value);
        Assert.Equal("Page", errors.FirstOrDefault().Key);
    }

    [Fact]
    public async Task GetAllServersAsync_ResultIsEmpty_ReturnsOkObjectResultWithEmptyData()
    {
        // Arrange
        GetServersQuery query = new(1, 10, "");
        _mockMediator.Setup(x => x.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<ServerDto>());

        // Act
        var result = await _controller.GetAllServersAsync(1, 10, "");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var servers = Assert.IsAssignableFrom<IEnumerable<ServerDto>>(okResult.Value);
        Assert.Empty(servers);
    }

    [Fact]
    public async Task GetAllServersAsync_ResultIsNotEmpty_ReturnsOkObjectResultWithData()
    {
        // Arrange
        GetServersQuery query = new(1, 10, "");
        _mockMediator.Setup(x => x.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ServerDto>()
            {
                new ServerDto(),
                new ServerDto(),
                new ServerDto()
            });

        // Act
        var result = await _controller.GetAllServersAsync(1, 10, "");

        // Assert
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
        CreateServerCommand command = new("Server Name", "Short Description", "Long Description", "Thumbnail") { };

        // Act
        var result = await _controller.CreateServerAsync(command);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task CreateServerAsync_ValidationResultIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        CreateServerCommand command = new("", "Short Description", "Long Description", "Thumbnail") { };
        _createServerCommandValidator.RuleFor(x => x.Name).Must(name => false);

        // Act
        var result = await _controller.CreateServerAsync(command);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<SerializableError>(badResult.Value);
        Assert.Equal("Name", errors.FirstOrDefault().Key);
    }

    [Fact]
    public async Task CreateServerAsync_ValidationResultIsValid_ReturnsOkObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        CreateServerCommand command = new("Server Name", "Short Description", "Long Description", "Thumbnail") { };
        _createServerCommandValidator.RuleFor(x => x.Name).Must(name => true);
        _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(It.IsAny<Guid>());

        // Act
        var result = await _controller.CreateServerAsync(command);

        // Assert
        _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
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
