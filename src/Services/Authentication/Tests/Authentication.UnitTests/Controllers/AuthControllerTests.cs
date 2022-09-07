using Authentication.Contracts;
using Authentication.Presentation.Controllers;
using Authentication.Presentation.Models;
using Authentication.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Authentication.UnitTests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IServiceManager> _mockServiceManager;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        Mock<IAuthenticationService> mockAuthenticationService = new();
        _mockServiceManager = new Mock<IServiceManager>();
        _mockServiceManager.Setup(x => x.AuthenticationService).Returns(mockAuthenticationService.Object);
        _controller = new AuthController(_mockServiceManager.Object, null);
    }

    [Fact]
    public async Task RegisterAsync_ModelStateIsInvalid_ReturnsBadRequest()
    {
        RegisterRequest request = new() { };
        _controller.ModelState.AddModelError("FirstName", "Required");

        var result = await _controller.RegisterAsync(request);

        _mockServiceManager.Verify(x => x.AuthenticationService.RegisterUserAsync(new UserDto()), Times.Never);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RegisterAsync_RequestIsValid_ReturnsOkResponse()
    {
        UserDto? newUser = null;
        _mockServiceManager.Setup(x => x.AuthenticationService.RegisterUserAsync(It.IsAny<UserDto>()))
            .Callback<UserDto>(x => newUser = x);

        var result = await _controller.RegisterAsync(new RegisterRequest());

        _mockServiceManager.Verify(x => x.AuthenticationService.RegisterUserAsync(newUser!), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task LoginAsync_ModelStateIsInvalid_ReturnsBadRequest()
    {
        LoginRequest request = new() { };
        _controller.ModelState.AddModelError("Email", "Required");

        var result = await _controller.LoginAsync(request);

        _mockServiceManager.Verify(x => x.AuthenticationService.LoginUserAsync(new UserDto()), Times.Never);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task LoginAsync_RequestIsValid_ReturnsOkResponse()
    {
        _mockServiceManager.Setup(x => x.AuthenticationService.LoginUserAsync(It.IsAny<UserDto>()))
            .ReturnsAsync(new TokenDto());

        var result = await _controller.LoginAsync(new LoginRequest());

        _mockServiceManager.Verify(x => x.AuthenticationService.LoginUserAsync(new UserDto()), Times.Once);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<TokenDto>(okResult.Value);
    }

    [Fact]
    public async Task RefreshTokenAsync_ModelStateIsInvalid_ReturnsBadRequest()
    {
        RefreshTokenRequest token = new();
        _controller.ModelState.AddModelError("Email", "Required");

        var result = await _controller.RefreshTokenAsync(token);

        _mockServiceManager.Verify(x => x.AuthenticationService.RefreshTokenAsync(new TokenDto()), Times.Never);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RefreshTokenAsync_RequestIsValid_ReturnsOkResponse()
    {
        _mockServiceManager.Setup(x => x.AuthenticationService.RefreshTokenAsync(It.IsAny<TokenDto>()))
            .ReturnsAsync(new TokenDto());

        var result = await _controller.RefreshTokenAsync(new RefreshTokenRequest());

        _mockServiceManager.Verify(x => x.AuthenticationService.RefreshTokenAsync(new TokenDto()), Times.Once);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<TokenDto>(okResult.Value);
    }
}
