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

    [HttpGet]
    [Route("{spaceId}/topics")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TopicDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Guid))]
    public async Task<IActionResult> GetAsync(Guid spaceId)
    {
        SpaceDto? space = await _serviceManager.SpaceService.GetByIdAsync(spaceId);
        if (space == null)
            return NotFound(spaceId);

        var result = await _serviceManager.SpaceService.GetAllTopicsAsync(spaceId);
        return Ok(result);
    }

    [HttpGet]
    [Route("{spaceSlug}/topics/{topicSlug}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TopicDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<IActionResult> GetAsync(string spaceSlug, string topicSlug)
    {
        TopicDto? topic = await _serviceManager.SpaceService.GetTopicBySlugAsync(spaceSlug, topicSlug);
        if (topic == null)
            return NotFound(topicSlug);

        return Ok(topic);
    }

    [HttpPost]
    [Route("{spaceId}/topics")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    public async Task<IActionResult> PostAsync(Guid spaceId, [FromBody] CreateEditTopicRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            return Unauthorized("Invalid user");

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

    [HttpPut]
    [Route("{spaceId}/topics/{topicId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    public async Task<IActionResult> PutAsync(Guid spaceId, Guid topicId, [FromBody] CreateEditTopicRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            return Unauthorized("Invalid user");

        TopicDto dto = new()
        {
            Id = topicId,
            AuthorEmail = User.Identity.Name,
            SpaceId = spaceId,
            Title = request.Title,
            Content = request.Content
        };
        await _serviceManager.SpaceService.UpdateTopicAsync(dto);

        return Ok("Topic has been updated");
    }

    [HttpDelete]
    [Route("{spaceId}/topics/{topicId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    public async Task<IActionResult> DeleteAsync(Guid spaceId, Guid topicId)
    {
        if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            return Unauthorized("Invalid user");

        TopicDto dto = new()
        {
            Id = topicId,
            SpaceId = spaceId,
            AuthorEmail = User.Identity.Name
        };
        await _serviceManager.SpaceService.DeleteTopicAsync(dto);

        return Ok("Topic has been deleted");
    }

    [HttpPost]
    [Route("{spaceId}/topics/{topicId}/upvote")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    public async Task<IActionResult> UpvoteAsync(Guid spaceId, Guid topicId)
    {
        if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            return Unauthorized("Invalid user");

        await _serviceManager.SpaceService.UpvoteTopicAsync(spaceId, topicId, User.Identity.Name);

        return Ok("Topic has been upvoted");
    }

    [HttpPost]
    [Route("{spaceId}/topics/{topicId}/downvote")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    public async Task<IActionResult> DownvoteAsync(Guid spaceId, Guid topicId)
    {
        if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            return Unauthorized("Invalid user");

        await _serviceManager.SpaceService.DownvoteTopicAsync(spaceId, topicId, User.Identity.Name);

        return Ok("Topic has been downvoted");
    }

    [HttpPost]
    [Route("{spaceId}/topics/{topicId}/unvote")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    public async Task<IActionResult> UnvoteAsync(Guid spaceId, Guid topicId)
    {
        if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            return Unauthorized("Invalid user");

        await _serviceManager.SpaceService.UnvoteTopicAsync(spaceId, topicId, User.Identity.Name);

        return Ok("Your vote has been removed");
    }
}
