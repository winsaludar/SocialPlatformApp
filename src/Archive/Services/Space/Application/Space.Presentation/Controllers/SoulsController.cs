using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Space.Contracts;
using Space.Services.Abstraction;

namespace Space.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SoulsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public SoulsController(IServiceManager serviceManager) => _serviceManager = serviceManager;

    [HttpGet]
    [Route("{soulId}/topics")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TopicDto>))]
    public async Task<IActionResult> GetAllTopicsAsync(Guid soulId)
    {
        var topics = await _serviceManager.SoulService.GetAllTopicsByIdAsync(soulId);
        return Ok(topics);
    }
}
