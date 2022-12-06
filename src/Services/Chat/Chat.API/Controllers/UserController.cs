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
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidatorManager _validatorManager;

    public UserController(IMediator mediator, IValidatorManager validatorManager)
    {
        _mediator = mediator;
        _validatorManager = validatorManager;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ServerDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [Route("servers")]
    public async Task<IActionResult> GetAllUserServersAsync()
    {
        User user = await GetUserAsync();
        GetUserServersQuery query = new(user.Id);
        ValidationResult validationResult = await _validatorManager.GetUserServersQueryValidator.ValidateAsync(query);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [Route("change-username")]
    public async Task<IActionResult> ChangeUsernameAsync([FromBody] ChangeUsernameModel request)
    {
        User user = await GetUserAsync();
        Server server = await GetServerAsync(request.ServerId);
        ChangeUsernameCommand command = new(server, user.Id, request.NewUsername);
        ValidationResult validationResult = await _validatorManager.ChangeUsernameCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        await _mediator.Send(command);

        return Ok("Your username is updated");
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
