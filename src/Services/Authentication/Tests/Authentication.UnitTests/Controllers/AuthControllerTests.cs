using Authentication.Contracts;
using Authentication.Domain.Entities;
using Authentication.Domain.Repositories;
using Authentication.Presentation.Controllers;
using Authentication.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Authentication.UnitTests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IRepositoryManager> _mockRepo;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        Mock<IUserRepository> mockUserRepo = new();
        Mock<ITokenRepository> mockTokenRepo = new();
        Mock<IRefreshTokenRepository> mockRefreshTokenRepo = new();
        _mockRepo = new Mock<IRepositoryManager>();
        _mockRepo.Setup(x => x.UserRepository).Returns(mockUserRepo.Object);
        _mockRepo.Setup(x => x.TokenRepository).Returns(mockTokenRepo.Object);
        _mockRepo.Setup(x => x.RefreshTokenRepository).Returns(mockRefreshTokenRepo.Object);
        _controller = new AuthController(_mockRepo.Object);
    }

    [Fact]
    public async Task RegisterAsync_ModelStateIsInvalid_ReturnsBadRequest()
    {
        RegisterRequest request = new() { };
        _controller.ModelState.AddModelError("FirstName", "Required");

        var result = await _controller.RegisterAsync(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RegisterAsync_RequestIsValid_ReturnsOkResponse()
    {
        RegisterRequest request = new()
        {
            FirstName = "First",
            LastName = "Last",
            Email = "test@example.com",
            Password = "password"
        };

        _mockRepo.Setup(x => x.UserRepository.GetByEmailAsync(request.Email))
            .ReturnsAsync((User)null!);
        _mockRepo.Setup(x => x.UserRepository.ValidateRegistrationPasswordAsync(request.Password))
            .ReturnsAsync(true);

        var result = await _controller.RegisterAsync(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task LoginAsync_ModelStateIsInvalid_ReturnsBadRequest()
    {
        LoginRequest request = new() { };
        _controller.ModelState.AddModelError("Email", "Required");

        var result = await _controller.LoginAsync(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task LoginAsync_RequestIsValid_ReturnsOkResponse()
    {
        LoginRequest request = new() { Email = "existingemail@example.com", Password = "password" };

        _mockRepo.Setup(x => x.UserRepository.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new User());
        _mockRepo.Setup(x => x.UserRepository.ValidateLoginPasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        _mockRepo.Setup(x => x.TokenRepository.GenerateJwtAsync(It.IsAny<User>(), null))
            .ReturnsAsync(new Token { Value = "fake-token", RefreshToken = "fake-refresh-token" });

        var result = await _controller.LoginAsync(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<TokenDto>(okResult.Value);
    }

    [Fact]
    public async Task RefreshTokenAsync_ModelStateIsInvalid_ReturnsBadRequest()
    {
        RefreshTokenRequest token = new();
        _controller.ModelState.AddModelError("Email", "Required");

        var result = await _controller.RefreshTokenAsync(token);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RefreshTokenAsync_RequestIsValid_ReturnsOkResponse()
    {
        RefreshTokenRequest request = new() { Token = "old-token", RefreshToken = "old-refresh-token" };

        _mockRepo.Setup(x => x.RefreshTokenRepository.GetByOldRefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new RefreshToken());
        _mockRepo.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new User());
        _mockRepo.Setup(x => x.TokenRepository.RefreshJwtAsync(It.IsAny<Token>(), It.IsAny<User>(), It.IsAny<RefreshToken>()))
            .ReturnsAsync(new Token());

        var result = await _controller.RefreshTokenAsync(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<TokenDto>(okResult.Value);
    }
}
