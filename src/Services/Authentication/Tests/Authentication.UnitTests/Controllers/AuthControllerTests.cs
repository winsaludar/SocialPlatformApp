using Authentication.API.Controllers;
using Authentication.API.Models;
using Authentication.Core.Contracts;
using Authentication.Core.Models;
using EventBus.Core.Abstractions;
using EventBus.Core.Events;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Authentication.UnitTests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IServiceManager> _mockServiceManager;
    private readonly Mock<IEventBus> _mockEventBus;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        Mock<IAuthService> mockAuthenticationService = new();
        _mockEventBus = new Mock<IEventBus>();
        _mockServiceManager = new Mock<IServiceManager>();
        _mockServiceManager.Setup(x => x.AuthenticationService).Returns(mockAuthenticationService.Object);
        _controller = new AuthController(_mockServiceManager.Object, _mockEventBus.Object);
    }

    [Fact]
    public async Task RegisterAsync_ModelStateIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        RegisterRequest request = new() { };
        _controller.ModelState.AddModelError("FirstName", "Required");

        // Act
        var result = await _controller.RegisterAsync(request);

        // Assert
        _mockServiceManager.Verify(x => x.AuthenticationService.RegisterUserAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RegisterAsync_RequestIsValid_ReturnsOkObjectResult()
    {
        // Arrange
        RegisterRequest request = new() { FirstName = "FirstName", LastName = "LastName", Email = "Email", Password = "Password" };
        _mockServiceManager.Setup(x => x.AuthenticationService.RegisterUserAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _controller.RegisterAsync(request);

        // Assert
        _mockServiceManager.Verify(x => x.AuthenticationService.RegisterUserAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        _mockEventBus.Verify(x => x.Publish(It.IsAny<IntegrationEvent>()), Times.Once);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var newId = Assert.IsType<Guid>(okResult.Value);
        Assert.NotEqual(Guid.Empty, newId);
    }

    [Fact]
    public async Task LoginAsync_ModelStateIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        LoginRequest request = new() { };
        _controller.ModelState.AddModelError("Email", "Required");

        // Act
        var result = await _controller.LoginAsync(request);

        // Assert
        _mockServiceManager.Verify(x => x.AuthenticationService.LoginUserAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task LoginAsync_RequestIsValid_ReturnsOkObjectResult()
    {
        // Arrange
        LoginRequest request = new() { Email = "user@example.com", Password = "password" };
        _mockServiceManager.Setup(x => x.AuthenticationService.LoginUserAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(new Token("fake-token", "fake-refresh-token", DateTime.Now));

        // Act
        var result = await _controller.LoginAsync(request);

        // Assert
        _mockServiceManager.Verify(x => x.AuthenticationService.LoginUserAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var token = Assert.IsType<Token>(okResult.Value);
        Assert.NotNull(token);
    }

    [Fact]
    public async Task RefreshTokenAsync_ModelStateIsInvalid_ReturnsBadRequestObjectResult()
    {
        // Arrange
        RefreshTokenRequest token = new();
        _controller.ModelState.AddModelError("Email", "Required");

        // Act
        var result = await _controller.RefreshTokenAsync(token);

        // Assert
        _mockServiceManager.Verify(x => x.AuthenticationService.RefreshTokenAsync(It.IsAny<Token>()), Times.Never);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RefreshTokenAsync_RequestIsValid_ReturnsOkObjectResult()
    {
        // Arrange
        RefreshTokenRequest request = new() { Token = "old-token", RefreshToken = "old-refresh-token" };
        _mockServiceManager.Setup(x => x.AuthenticationService.RefreshTokenAsync(It.IsAny<Token>()))
            .ReturnsAsync(new Token("fake-token", "fake-refresh-token", DateTime.Now));

        // Act
        var result = await _controller.RefreshTokenAsync(request);

        // Assert
        _mockServiceManager.Verify(x => x.AuthenticationService.RefreshTokenAsync(It.IsAny<Token>()), Times.Once);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var token = Assert.IsType<Token>(okResult.Value);
        Assert.NotNull(token);
    }
}
