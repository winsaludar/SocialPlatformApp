using Chat.API.Extensions;
using Chat.API.Models;
using Chat.Application.Commands;
using Chat.Application.Queries;
using Chat.Application.Validators;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Aggregates.UserAggregate;
using Chat.Domain.Exceptions;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers;

[ApiController]
[Route("api/servers")]
[Authorize]
public class ModeratorsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidatorManager _validatorManager;

    public ModeratorsController(IMediator mediator, IValidatorManager validatorManager)
    {
        _mediator = mediator;
        _validatorManager = validatorManager;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [Route("{serverId}/moderators/add")]
    public async Task<IActionResult> AddModeratorAsync(Guid serverId, [FromBody] AddRemoveModeratorModel request)
    {
        User currentUser = await GetUserAsync();
        Server server = await GetServerAsync(serverId);
        AddModeratorCommand command = new(server, request.UserId, currentUser.Id);
        ValidationResult validationResult = await _validatorManager.AddModeratorCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        await _mediator.Send(command);

        return Ok("User is now a moderator of the server");
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [Route("{serverId}/moderators/remove")]
    public async Task<IActionResult> RemoveModeratorAsync(Guid serverId, [FromBody] AddRemoveModeratorModel request)
    {
        User currentUser = await GetUserAsync();
        Server server = await GetServerAsync(serverId);
        RemoveModeratorCommand command = new(server, request.UserId, currentUser.Id);
        //ValidationResult validationResult = await _validatorManager.AddModeratorCommandValidator.ValidateAsync(command);
        //if (!validationResult.IsValid)
        //{
        //    validationResult.AddToModelState(ModelState);
        //    return BadRequest(ModelState);
        //}

        await _mediator.Send(command);

        return Ok("User has been downgraded to member");
    }

    private async Task<User> GetUserAsync()
    {
        if (!User.IsValid())
            throw new UnauthorizedAccessException("User is invalid");

        GetUserByEmailQuery query = new(User.Identity!.Name!);
        var user = await _mediator.Send(query);
        if (user is null)
            throw new UnauthorizedAccessException($"User '{User.Identity!.Name!}' does not exist");

        return user;
    }

    private async Task<Server> GetServerAsync(Guid serverId)
    {
        GetServerQuery query = new(serverId);
        Server? server = await _mediator.Send(query);
        if (server is null)
            throw new ServerNotFoundException(serverId.ToString());

        return server;
    }
}
