﻿using Authentication.Contracts;
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
        var mockApplicationUserService = new Mock<IApplicationUserService>();
        _mockService = new Mock<IServiceManager>();
        _mockService.SetupGet(x => x.ApplicationUserService).Returns(mockApplicationUserService.Object);
        _controller = new AuthController(_mockService.Object);
    }

    [Fact]
    public async Task RegisterAsync_InvalidModelState_ReturnsBadRequest()
    {
        RegisterApplicationUser user = new() { };
        _controller.ModelState.AddModelError("FirstName", "Required");

        var result = await _controller.RegisterAsync(user);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RegisterAsync_ValidModelState_ReturnsOkResponse()
    {
        RegisterApplicationUserDto? createdUser = null;
        _mockService.Setup(x => x.ApplicationUserService.RegisterAsync(It.IsAny<RegisterApplicationUserDto>()))
            .Callback<RegisterApplicationUserDto>(x => createdUser = x);

        RegisterApplicationUser newUser = new()
        {
            FirstName = "First",
            LastName = "Last",
            Email = "test@example.com",
            Password = "password"
        };
        var result = await _controller.RegisterAsync(newUser);

        _mockService.Verify(x => x.ApplicationUserService.RegisterAsync(It.IsAny<RegisterApplicationUserDto>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task LoginAsync_InvalidModelState_ReturnsBadRequest()
    {
        LoginApplicationUser user = new() { };
        _controller.ModelState.AddModelError("Email", "Required");

        var result = await _controller.LoginAsync(user);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task LoginAsync_ValidModelState_ReturnsOkResponse()
    {
        LoginApplicationUser user = new() { Email = "existingemail@example.com", Password = "password" };
        _mockService.Setup(x => x.ApplicationUserService.LoginAsync(It.IsAny<LoginUserDto>()))
            .ReturnsAsync(new TokenDto { Token = "fake-token" });

        var result = await _controller.LoginAsync(user);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<TokenDto>(okResult.Value);
    }
}
