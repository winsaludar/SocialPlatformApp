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
public class SpaceTopicCommentsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public SpaceTopicCommentsController(IServiceManager serviceManager) => _serviceManager = serviceManager;

    [HttpPost]
    [Route("{spaceId}/topics/{topicId}/comments")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    public async Task<IActionResult> PostAsync(Guid spaceId, Guid topicId, [FromBody] CreateEditCommentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            return Unauthorized("Invalid user");

        CommentDto dto = new()
        {
            SpaceId = spaceId,
            TopicId = topicId,
            AuthorEmail = User.Identity.Name,
            Content = request.Content
        };
        await _serviceManager.SpaceService.CreateCommentAsync(dto);

        return Ok("Your comment has been posted");
    }
}
