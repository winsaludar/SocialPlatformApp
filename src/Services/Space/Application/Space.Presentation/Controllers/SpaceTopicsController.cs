﻿using Microsoft.AspNetCore.Authorization;
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(Guid spaceId)
    {
        SpaceDto? space = await _serviceManager.SpaceService.GetByIdAsync(spaceId);
        if (space == null)
            return NotFound(spaceId);

        var result = await _serviceManager.SpaceService.GetAllTopicsAsync(spaceId);
        return Ok(result);
    }

    [HttpPost]
    [Route("{spaceId}/topics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PostAsync(Guid spaceId, [FromBody] CreateEditSpaceTopicRequest request)
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

    [HttpPut]
    [Route("{spaceId}/topics/{topicId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PutAsync(Guid spaceId, Guid topicId, [FromBody] CreateEditSpaceTopicRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            return Unauthorized();

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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteAsync(Guid spaceId, Guid topicId)
    {
        if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            return Unauthorized();

        TopicDto dto = new()
        {
            Id = topicId,
            SpaceId = spaceId,
            AuthorEmail = User.Identity.Name
        };
        await _serviceManager.SpaceService.DeleteTopicAsync(dto);

        return Ok("Topic has been deleted");
    }
}