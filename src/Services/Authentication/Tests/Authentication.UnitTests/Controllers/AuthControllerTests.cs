using Authentication.Contracts;
using Authentication.Presentation.Controllers;
using Authentication.Presentation.Models;
using Authentication.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Authentication.UnitTests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IServiceManager> _mockService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        Mock<IApplicationUserService> mockApplicationUserService = new();
        Mock<ITokenService> mockTokenService = new();
        _mockService = new Mock<IServiceManager>();
        _mockService.SetupGet(x => x.ApplicationUserService).Returns(mockApplicationUserService.Object);
        _mockService.SetupGet(x => x.TokenService).Returns(mockTokenService.Object);
        _controller = new AuthController(_mockService.Object);
    }

    [Fact]
    public async Task RegisterAsync_InvalidModelState_ReturnsBadRequest()
    {
        RegisterRequest user = new() { };
        _controller.ModelState.AddModelError("FirstName", "Required");

        var result = await _controller.RegisterAsync(user);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RegisterAsync_ValidModelState_ReturnsOkResponse()
    {
        RegisterUserDto? createdUser = null;
        _mockService.Setup(x => x.ApplicationUserService.RegisterAsync(It.IsAny<RegisterUserDto>()))
            .Callback<RegisterUserDto>(x => createdUser = x);

        RegisterRequest newUser = new()
        {
            FirstName = "First",
            LastName = "Last",
            Email = "test@example.com",
            Password = "password"
        };
        var result = await _controller.RegisterAsync(newUser);

        _mockService.Verify(x => x.ApplicationUserService.RegisterAsync(It.IsAny<RegisterUserDto>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task LoginAsync_InvalidModelState_ReturnsBadRequest()
    {
        LoginRequest user = new() { };
        _controller.ModelState.AddModelError("Email", "Required");

        var result = await _controller.LoginAsync(user);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task LoginAsync_ValidModelState_ReturnsOkResponse()
    {
        LoginRequest user = new() { Email = "existingemail@example.com", Password = "password" };
        _mockService.Setup(x => x.ApplicationUserService.LoginAsync(It.IsAny<LoginUserDto>()))
            .ReturnsAsync(new TokenDto { Token = "fake-token" });

        var result = await _controller.LoginAsync(user);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<TokenDto>(okResult.Value);
    }

    [Fact]
    public async Task RefreshTokenAsync_InvalidModelState_ReturnsBadRequest()
    {
        RefreshTokenRequest token = new();
        _controller.ModelState.AddModelError("Email", "Required");

        var result = await _controller.RefreshTokenAsync(token);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RefreshTokenAsync_ValidModelState_ReturnsOkResponse()
    {
        RefreshTokenRequest token = new() { Token = "fake-token", RefreshToken = "fake-refresh-token" };
        _mockService.Setup(x => x.TokenService.RefreshJwtAsync(It.IsAny<TokenDto>()))
            .ReturnsAsync(new TokenDto());

        var result = await _controller.RefreshTokenAsync(token);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<TokenDto>(okResult.Value);
    }
}
