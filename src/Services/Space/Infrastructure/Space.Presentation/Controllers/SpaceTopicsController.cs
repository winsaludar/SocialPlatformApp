using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Space.Contracts;
using Space.Presentation.Models;
using Space.Services.Abstraction;

namespace Space.Presentation.Controllers;

[ApiController]
[Route("api/spaces")]
[Authorize]
public class SpaceTopicsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public SpaceTopicsController(IServiceManager serviceManager) => _serviceManager = serviceManager;

    [HttpPost]
    [Route("{spaceId}/topics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PostAsync(Guid spaceId, [FromBody] CreateSpaceTopicRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            return Unauthorized();

        TopicDto dto = new()
        {
            AuthorEmail = User.Identity.Name,
            SpaceId = spaceId,
            Title = request.Title,
            Content = request.Content
        };
        await _serviceManager.SpaceService.CreateTopicAsync(dto);

        return Ok("Your topic has been created");
    }
}
