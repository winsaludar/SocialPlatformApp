using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Space.Contracts;
using Space.Presentation.Controllers;
using Space.Presentation.Models;
using Space.Services.Abstraction;
using System.Security.Claims;

namespace Space.UnitTests.Controllers;

public class SpaceControllerTests
{
    private readonly Mock<IServiceManager> _mockService;
    private readonly SpacesController _controller;

    public SpaceControllerTests()
    {
        Mock<ISpaceService> mockSpaceService = new();
        _mockService = new Mock<IServiceManager>();
        _mockService.Setup(x => x.SpaceService).Returns(mockSpaceService.Object);
        _controller = new SpacesController(_mockService.Object);
    }

    [Fact]
    public async Task PostAsync_ModelStateIsInvalid_ReturnsBadRequest()
    {
        CreateSpaceRequest request = new() { };
        _controller.ModelState.AddModelError("Name", "Required");

        var result = await _controller.PostAsync(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PostAsync_UserIdentityIsNull_ReturnsUnauthorized()
    {
        // Setup a null User.Identity
        Mock<ClaimsPrincipal> user = new();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user.Object }
        };
        CreateSpaceRequest request = new()
        {
            Name = "Test",
            ShortDescription = "Short Desc",
            LongDescription = "Long Desc"
        };

        var result = await _controller.PostAsync(request);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task PostAsync_RequestIsValid_ReturnsOkResponse()
    {
        SpaceDto? createdSpace = null;
        _mockService.Setup(x => x.SpaceService.CreateAsync(It.IsAny<SpaceDto>()))
            .Callback<SpaceDto>(x => createdSpace = x);

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

        CreateSpaceRequest request = new()
        {
            Name = "Test",
            ShortDescription = "Short Desc",
            LongDescription = "Long Desc"
        };

        var result = await _controller.PostAsync(request);

        _mockService.Verify(x => x.SpaceService.CreateAsync(It.IsAny<SpaceDto>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(createdSpace?.Name, request.Name);
        Assert.Equal(createdSpace?.ShortDescription, request.ShortDescription);
        Assert.Equal(createdSpace?.LongDescription, request.LongDescription);
    }
}
