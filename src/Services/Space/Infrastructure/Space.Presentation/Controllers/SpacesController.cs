using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Space.Contracts;
using Space.Presentation.Models;
using Space.Services.Abstraction;

namespace Space.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SpacesController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public SpacesController(IServiceManager serviceManager) => _serviceManager = serviceManager;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SpaceDto>))]
    public async Task<IActionResult> GetAsync()
    {
        var result = await _serviceManager.SpaceService.GetAllAsync();
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PostAsync([FromBody] CreateSpaceRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            return Unauthorized();

        var dto = request.Adapt<SpaceDto>();
        dto.Creator = User.Identity.Name;
        await _serviceManager.SpaceService.CreateAsync(dto);

        return Ok("Space created");
    }

    [HttpPost]
    [Route("{spaceId}/join")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> JoinSpaceAsync(Guid spaceId)
    {
        if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            return Unauthorized();

        await _serviceManager.SoulService.JoinSpaceAsync(spaceId, User.Identity.Name);

        return Ok("Welcome new soul!");
    }
}
