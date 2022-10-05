using Chat.API.Extensions;
using Chat.API.Models;
using Chat.Application.Commands;
using Chat.Application.DTOs;
using Chat.Application.Queries;
using Chat.Application.Validators;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers;

[ApiController]
[Route("api/servers")]
[Authorize]
public class ChannelsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidatorManager _validatorManager;

    public ChannelsController(IMediator mediator, IValidatorManager validatorManager)
    {
        _mediator = mediator;
        _validatorManager = validatorManager;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ChannelDto>))]
    [Route("{serverId}/channels")]
    public async Task<IActionResult> GetAllChannelsAsync(Guid serverId)
    {
        GetChannelsQuery query = new(serverId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [Route("{serverId}/channels")]
    public async Task<IActionResult> CreateChannelAsync(Guid serverId, [FromBody] CreateUpdateChannelModel request)
    {
        if (!User.IsValid())
            return Unauthorized("User is invalid");

        CreateChannelCommand command = new(serverId, request.Name);
        ValidationResult validationResult = await _validatorManager.CreateChannelCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        var result = await _mediator.Send(command);
        return Ok(new { id = result, message = "Channel created" });
    }
}
