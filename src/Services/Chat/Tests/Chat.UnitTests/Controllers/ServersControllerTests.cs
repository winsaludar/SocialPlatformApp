using Chat.API.Controllers;
using Chat.Application.Commands;
using Chat.Application.DTOs;
using Chat.Application.Queries;
using Chat.Application.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

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
    public async Task GetAllAsync_ValidationResultIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        GetServersQuery query = new(0, 10, "");
        _getServersQueryValidator.RuleFor(x => x.Page).Must(page => false);

        // Act
        var result = await _controller.GetAllAsync(0, 10, "");

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<SerializableError>(badResult.Value);
        Assert.Equal("Page", errors.FirstOrDefault().Key);
    }

    [Fact]
    public async Task GetAllAsync_ResultIsEmpty_ReturnsOkObjectResultWithEmptyData()
    {
        // Arrange
        GetServersQuery query = new(1, 10, "");
        _mockMediator.Setup(x => x.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<ServerDto>());

        // Act
        var result = await _controller.GetAllAsync(1, 10, "");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var servers = Assert.IsAssignableFrom<IEnumerable<ServerDto>>(okResult.Value);
        Assert.Empty(servers);
    }

    [Fact]
    public async Task GetAllAsync_ResultIsNotEmpty_ReturnsOkObjectResultWithData()
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
        var result = await _controller.GetAllAsync(1, 10, "");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var servers = Assert.IsAssignableFrom<IEnumerable<ServerDto>>(okResult.Value);
        Assert.NotEmpty(servers);
        Assert.Equal(3, servers.Count());
    }

    [Fact]
    public async Task PostAsync_ValidationResultIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        CreateServerCommand command = new("", "Short Description", "Long Description", "Thumbnail") { };
        _createServerCommandValidator.RuleFor(x => x.Name).Must(name => false);

        // Act
        var result = await _controller.PostAsync(command);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<SerializableError>(badResult.Value);
        Assert.Equal("Name", errors.FirstOrDefault().Key);
    }

    [Fact]
    public async Task PostAsync_ValidationResultIsValid_ReturnsOkObjectResult()
    {
        // Arrange
        CreateServerCommand command = new("Server Name", "Short Description", "Long Description", "Thumbnail") { };
        _createServerCommandValidator.RuleFor(x => x.Name).Must(name => true);
        _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(It.IsAny<Guid>());

        // Act
        var result = await _controller.PostAsync(command);

        // Assert
        _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }
}
