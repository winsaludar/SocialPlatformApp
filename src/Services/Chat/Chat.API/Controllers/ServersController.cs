﻿using Chat.API.Extensions;
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
        if (!User.IsValid())
            return Unauthorized("User is invalid");

        CreateServerCommand command = new(request.Name, request.ShortDescription, request.LongDescription, User.Identity.Name, request.Thumbnail);
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
        if (!User.IsValid())
            return Unauthorized("User is invalid");

        Server server = await GetServerAsync(serverId);
        UpdateServerCommand command = new(server, request.Name, request.ShortDescription, request.LongDescription, User.Identity!.Name!, request.Thumbnail);
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
        if (!User.IsValid())
            return Unauthorized("User is invalid");

        DeleteServerCommand command = new(serverId, User.Identity!.Name!);
        ValidationResult validationResult = await _validatorManager.DeleteServerCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        await _mediator.Send(command);

        return Ok("Server deleted");
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
