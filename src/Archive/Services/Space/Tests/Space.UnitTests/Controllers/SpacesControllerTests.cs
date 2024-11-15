﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Space.Contracts;
using Space.Presentation.Controllers;
using Space.Presentation.Models;
using Space.Services.Abstraction;
using System.Security.Claims;

namespace Space.UnitTests.Controllers;

public class SpacesControllerTests
{
    private readonly Mock<IServiceManager> _mockService;
    private readonly SpacesController _controller;

    public SpacesControllerTests()
    {
        Mock<ISpaceService> mockSpaceService = new();
        Mock<ISoulService> mockSoulService = new();
        _mockService = new Mock<IServiceManager>();
        _mockService.Setup(x => x.SpaceService).Returns(mockSpaceService.Object);
        _mockService.Setup(x => x.SoulService).Returns(mockSoulService.Object);
        _controller = new SpacesController(_mockService.Object);
    }

    [Fact]
    public async Task GetAsync_SpacesAreEmpty_ReturnsOkResultWithEmptyData()
    {
        _mockService.Setup(x => x.SpaceService.GetAllAsync())
           .ReturnsAsync((IEnumerable<SpaceDto>)new List<SpaceDto>());

        var result = await _controller.GetAsync();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var spaces = Assert.IsType<List<SpaceDto>>(okResult.Value);
        Assert.Empty(spaces);
    }

    [Fact]
    public async Task GetAsync_SpacesAreNotEmpty_ReturnsOkResultWithData()
    {
        _mockService.Setup(x => x.SpaceService.GetAllAsync())
           .ReturnsAsync((IEnumerable<SpaceDto>)new List<SpaceDto>
           {
               new SpaceDto(),
               new SpaceDto(),
               new SpaceDto()
           });

        var result = await _controller.GetAsync();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var spaces = Assert.IsType<List<SpaceDto>>(okResult.Value);
        Assert.NotEmpty(spaces);
        Assert.Equal(3, spaces.Count());
    }

    [Fact]
    public async Task GetBySlugAsync_SlugIsInvalid_ReturnsNotFound()
    {
        _mockService.Setup(x => x.SpaceService.GetBySlugAsync(It.IsAny<string>()))
           .ReturnsAsync((SpaceDto)null!);

        var result = await _controller.GetBySlugAsync(It.IsAny<string>());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetBySlugAsync_SlugIsValid_ReturnsOkResultWithData()
    {
        _mockService.Setup(x => x.SpaceService.GetBySlugAsync(It.IsAny<string>()))
           .ReturnsAsync(new SpaceDto());

        var result = await _controller.GetBySlugAsync(It.IsAny<string>());

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<SpaceDto>(okResult.Value);
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

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task PostAsync_RequestIsValid_ReturnsOkResponse()
    {
        SpaceDto? createdSpace = null;
        _mockService.Setup(x => x.SoulService.CreateSpaceAsync(It.IsAny<SpaceDto>()))
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

        _mockService.Verify(x => x.SoulService.CreateSpaceAsync(It.IsAny<SpaceDto>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(createdSpace?.Name, request.Name);
        Assert.Equal(createdSpace?.ShortDescription, request.ShortDescription);
        Assert.Equal(createdSpace?.LongDescription, request.LongDescription);
    }

    [Fact]
    public async Task JoinSpaceAsync_UserIdentityIsNull_ReturnsUnauthorized()
    {
        // Setup a null User.Identity
        Mock<ClaimsPrincipal> user = new();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user.Object }
        };
        Guid spaceId = Guid.NewGuid();

        var result = await _controller.JoinSpaceAsync(spaceId);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task JoinSpaceAsync_RequestIsValid_ReturnsOkResponse()
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
        Guid spaceId = Guid.NewGuid();

        var result = await _controller.JoinSpaceAsync(spaceId);

        _mockService.Verify(x => x.SoulService.JoinSpaceAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task LeaveSpaceAsync_UserIdentityIsNull_ReturnsUnauthorized()
    {
        // Setup a null User.Identity
        Mock<ClaimsPrincipal> user = new();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user.Object }
        };
        Guid spaceId = Guid.NewGuid();

        var result = await _controller.LeaveSpaceAsync(spaceId);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task LeaveSpaceAsync_RequestIsValid_ReturnsOkResponse()
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
        Guid spaceId = Guid.NewGuid();

        var result = await _controller.LeaveSpaceAsync(spaceId);

        _mockService.Verify(x => x.SoulService.LeaveSpaceAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task KickSoulAsync_UserIdentityIsNull_ReturnsUnauthorized()
    {
        // Setup a null User.Identity
        Mock<ClaimsPrincipal> user = new();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user.Object }
        };
        Guid spaceId = Guid.NewGuid();
        KickSpaceMemberRequest request = new();

        var result = await _controller.KickMemberAsync(spaceId, request);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task KickSoulAsync_RequestIsValid_ReturnsOkResponse()
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
        Guid spaceId = Guid.NewGuid();
        KickSpaceMemberRequest request = new() { Email = "member@example.com" };

        var result = await _controller.KickMemberAsync(spaceId, request);

        _mockService.Verify(x => x.SpaceService.KickMemberAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetAllMembersAsync_SpaceIdIsInvalid_ReturnsNotFoundResult()
    {
        _mockService.Setup(x => x.SpaceService.GetByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync((SpaceDto)null!);

        var result = await _controller.GetAllMembersAsync(It.IsAny<Guid>());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetAllMembersAsync_SoulsAreEmpty_ReturnsOkResultWithEmptyData()
    {
        _mockService.Setup(x => x.SpaceService.GetByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync(new SpaceDto());
        _mockService.Setup(x => x.SpaceService.GetAllMembersAsync(It.IsAny<Guid>()))
           .ReturnsAsync((IEnumerable<SoulDto>)new List<SoulDto>());

        var result = await _controller.GetAllMembersAsync(It.IsAny<Guid>());

        var okResult = Assert.IsType<OkObjectResult>(result);
        var spaces = Assert.IsType<List<SoulDto>>(okResult.Value);
        Assert.Empty(spaces);
    }

    [Fact]
    public async Task GetAllMembersAsync_SoulsAreNotEmpty_ReturnsOkResultWithData()
    {
        _mockService.Setup(x => x.SpaceService.GetByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync(new SpaceDto());
        _mockService.Setup(x => x.SpaceService.GetAllMembersAsync(It.IsAny<Guid>()))
           .ReturnsAsync((IEnumerable<SoulDto>)new List<SoulDto>
           {
               new SoulDto(),
               new SoulDto(),
               new SoulDto()
           });

        var result = await _controller.GetAllMembersAsync(It.IsAny<Guid>());

        var okResult = Assert.IsType<OkObjectResult>(result);
        var souls = Assert.IsType<List<SoulDto>>(okResult.Value);
        Assert.NotEmpty(souls);
        Assert.Equal(3, souls.Count());
    }

    [Fact]
    public async Task GetAllModeratorsAsync_SpaceIdIsInvalid_ReturnsNotFoundResult()
    {
        _mockService.Setup(x => x.SpaceService.GetByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync((SpaceDto)null!);

        var result = await _controller.GetAllModeratorsAsync(It.IsAny<Guid>());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetAllModeratorsAsync_SoulsAreEmpty_ReturnsOkResultWithEmptyData()
    {
        _mockService.Setup(x => x.SpaceService.GetByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync(new SpaceDto());
        _mockService.Setup(x => x.SpaceService.GetAllModeratorsAsync(It.IsAny<Guid>()))
           .ReturnsAsync((IEnumerable<SoulDto>)new List<SoulDto>());

        var result = await _controller.GetAllModeratorsAsync(It.IsAny<Guid>());

        var okResult = Assert.IsType<OkObjectResult>(result);
        var spaces = Assert.IsType<List<SoulDto>>(okResult.Value);
        Assert.Empty(spaces);
    }

    [Fact]
    public async Task GetAllModeratorsAsync_SoulsAreNotEmpty_ReturnsOkResultWithData()
    {
        _mockService.Setup(x => x.SpaceService.GetByIdAsync(It.IsAny<Guid>()))
           .ReturnsAsync(new SpaceDto());
        _mockService.Setup(x => x.SpaceService.GetAllModeratorsAsync(It.IsAny<Guid>()))
           .ReturnsAsync((IEnumerable<SoulDto>)new List<SoulDto>
           {
               new SoulDto(),
               new SoulDto(),
               new SoulDto()
           });

        var result = await _controller.GetAllModeratorsAsync(It.IsAny<Guid>());

        var okResult = Assert.IsType<OkObjectResult>(result);
        var souls = Assert.IsType<List<SoulDto>>(okResult.Value);
        Assert.NotEmpty(souls);
        Assert.Equal(3, souls.Count());
    }
}
