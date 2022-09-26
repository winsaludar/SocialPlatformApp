using Chat.API.Extensions;
using Chat.API.Models;
using Chat.Application.Commands;
using Chat.Application.DTOs;
using Chat.Application.Queries;
using Chat.Application.Validators;
using FluentValidation;
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
    public async Task<IActionResult> CreateServerAsync([FromBody] CreateServerCommand command)
    {
        if (User == null || User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            return Unauthorized("User is invalid");

        command.SetCreatorEmail(User.Identity.Name);

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
    [Route("{serverId}")]
    public async Task<IActionResult> UpdateServerAsync(Guid serverId, [FromBody] UpdateServerModel request)
    {
        if (User == null || User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            return Unauthorized("User is invalid");

        UpdateServerCommand command = new(serverId, request.Name, request.ShortDescription, request.LongDescription, User.Identity.Name, request.Thumbnail);
        ValidationResult validationResult = await _validatorManager.UpdateServerCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        await _mediator.Send(command);

        return Ok("Server updated");
    }
}
