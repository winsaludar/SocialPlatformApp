using Mapster;
using Microsoft.AspNetCore.Authorization;
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

    [HttpPost]
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
}
