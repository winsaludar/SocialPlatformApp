using Chat.API.Controllers;
using Chat.Application.Commands;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Chat.UnitTests.Controllers;

public class ServersControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly InlineValidator<CreateServerCommand> _validator;
    private readonly ServersController _controller;

    public ServersControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _validator = new InlineValidator<CreateServerCommand>();
        _controller = new ServersController(_mockMediator.Object, _validator);
    }

    [Fact]
    public async Task PostAsync_ValidationResultIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        CreateServerCommand command = new("", "Short Description", "Long Description", "Thumbnail") { };
        _validator.RuleFor(x => x.Name).Must(name => false);

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
        _validator.RuleFor(x => x.Name).Must(name => true);
        _mockMediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(It.IsAny<Guid>());

        // Act
        var result = await _controller.PostAsync(command);

        // Assert
        _mockMediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }
}
