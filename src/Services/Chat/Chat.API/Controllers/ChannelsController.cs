using Chat.API.Extensions;
using Chat.API.Models;
using Chat.Application.Commands;
using Chat.Application.DTOs;
using Chat.Application.Queries;
using Chat.Application.Validators;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
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
        ValidationResult validationResult = await _validatorManager.GetChannelsQueryValidator.ValidateAsync(query);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [Route("{serverId}/channels")]
    public async Task<IActionResult> CreateChannelAsync(Guid serverId, [FromBody] CreateUpdateChannelModel request)
    {
        if (!User.IsValid())
            return Unauthorized("User is invalid");

        GetServerQuery getServerQuery = new(serverId);
        Server? server = await _mediator.Send(getServerQuery);
        if (server is null)
            throw new ServerNotFoundException(serverId.ToString());

        CreateChannelCommand command = new(server, request.Name, User.Identity!.Name!);
        ValidationResult validationResult = await _validatorManager.CreateChannelCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        var result = await _mediator.Send(command);
        return Ok(new { id = result, message = "Channel created" });
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [Route("{serverId}/channels/{channelId}")]
    public async Task<IActionResult> UpdateChannelAsync(Guid serverId, Guid channelId, [FromBody] CreateUpdateChannelModel request)
    {
        if (!User.IsValid())
            return Unauthorized("User is invalid");

        UpdateChannelCommand command = new(serverId, channelId, request.Name, User.Identity!.Name!);
        ValidationResult validationResult = await _validatorManager.UpdateChannelCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        await _mediator.Send(command);

        return Ok("Channel updated");
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [Route("{serverId}/channels/{channelId}")]
    public async Task<IActionResult> DeleteChannelAsync(Guid serverId, Guid channelId)
    {
        DeleteChannelCommand command = new(serverId, channelId);
        ValidationResult validationResult = await _validatorManager.DeleteChannelCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        await _mediator.Send(command);

        return Ok("Channel deleted");
    }
}
