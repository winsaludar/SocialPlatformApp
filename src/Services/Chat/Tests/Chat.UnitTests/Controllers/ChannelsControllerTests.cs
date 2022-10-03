﻿using Chat.API.Controllers;
using Chat.API.Models;
using Chat.Application.Commands;
using Chat.Application.Validators;
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
    private readonly InlineValidator<CreateChannelCommand> _createChannelCommandValidator;
    private ChannelsController _controller;

    public ChannelsControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _createChannelCommandValidator = new InlineValidator<CreateChannelCommand>();

        Mock<IValidatorManager> mockValidatorManager = new();
        mockValidatorManager.Setup(x => x.CreateChannelCommandValidator).Returns(_createChannelCommandValidator);

        _controller = new ChannelsController(_mockMediator.Object, mockValidatorManager.Object);
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
    public async Task CreateChannelAsync_ValidationResultIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        SetUpFakeUserIdentity();
        Guid serverId = Guid.NewGuid();
        CreateUpdateChannelModel request = new() { Name = "" };
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
        _createChannelCommandValidator.RuleFor(x => x.Name).Must(name => true);
        _mockMediator.Setup(x => x.Send(It.IsAny<CreateChannelCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(It.IsAny<Guid>());

        // Act
        var result = await _controller.CreateChannelAsync(serverId, request);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<CreateChannelCommand>(), It.IsAny<CancellationToken>()), Times.Once);
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
