using Chat.API.Extensions;
using Chat.API.Models;
using Chat.Application.Commands;
using Chat.Application.DTOs;
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
[Route("api/[controller]")]
[Authorize]
public class ServersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidatorManager _validatorManager;

    public ServersController(IMediator mediator, IValidatorManager validatorManager)
    {
        _mediator = mediator;
        _validatorManager = validatorManager;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ServerDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> GetAllServersAsync(int page = 1, int size = 10, string? name = null)
    {
        GetServersQuery query = new(page, size, name);
        ValidationResult validationResult = await _validatorManager.GetServersQueryValidator.ValidateAsync(query);
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
    public async Task<IActionResult> CreateServerAsync([FromBody] CreateUpdateServerModel request)
    {
        User user = await GetUserAsync();
        CreateServerCommand command = new(request.Name, request.ShortDescription, request.LongDescription, User.Identity!.Name!, user.Id, request.Thumbnail);
        ValidationResult validationResult = await _validatorManager.CreateServerCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        var result = await _mediator.Send(command);

        return Ok(new { Id = result, message = "Server created" });
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [Route("{serverId}")]
    public async Task<IActionResult> UpdateServerAsync(Guid serverId, [FromBody] CreateUpdateServerModel request)
    {
        User user = await GetUserAsync();
        Server server = await GetServerAsync(serverId);
        UpdateServerCommand command = new(server, request.Name, request.ShortDescription, request.LongDescription, user.Id, request.Thumbnail);
        ValidationResult validationResult = await _validatorManager.UpdateServerCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        await _mediator.Send(command);

        return Ok("Server updated");
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [Route("{serverId}")]
    public async Task<IActionResult> DeleteServerAsync(Guid serverId)
    {
        User user = await GetUserAsync();
        DeleteServerCommand command = new(serverId, user.Id);
        ValidationResult validationResult = await _validatorManager.DeleteServerCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        await _mediator.Send(command);

        return Ok("Server deleted");
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [Route("{serverId}/join")]
    public async Task<IActionResult> JoinServerAsync(Guid serverId)
    {
        User user = await GetUserAsync();
        Server server = await GetServerAsync(serverId);
        JoinServerCommand command = new(server, user.Id, user.Username);

        await _mediator.Send(command);

        return Ok("Welcome to the server");
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
