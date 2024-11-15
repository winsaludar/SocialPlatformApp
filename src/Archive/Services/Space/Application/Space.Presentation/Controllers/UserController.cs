﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Space.Contracts;
using Space.Services.Abstraction;

namespace Space.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public UserController(IServiceManager serviceManager) => _serviceManager = serviceManager;

    [HttpGet]
    [Route("moderated-spaces")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SpaceDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllModeratedSpacesAsync()
    {
        if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            return Unauthorized("Invalid user");

        var spaces = await _serviceManager.SoulService.GetAllModeratedSpacesByEmailAsync(User.Identity.Name);
        return Ok(spaces);
    }

    [HttpGet]
    [Route("topics")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TopicDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllTopicsAsync()
    {
        if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            return Unauthorized("Invalid user");

        var topics = await _serviceManager.SoulService.GetAllTopicsByEmailAsync(User.Identity.Name);
        return Ok(topics);
    }

    [HttpGet]
    [Route("comments")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TopicDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllCommentsAsync()
    {
        if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            return Unauthorized("Invalid user");

        var topics = await _serviceManager.SoulService.GetAllCommentsByEmailAsync(User.Identity.Name);
        return Ok(topics);
    }
}
