using Chat.API.Extensions;
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
    public async Task<IActionResult> GetAllAsync(int page = 1, int size = 10, string? name = null)
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> PostAsync([FromBody] CreateServerCommand command)
    {
        ValidationResult validationResult = await _validatorManager.CreateServerCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        var result = await _mediator.Send(command);

        return Ok(result.ToString());
    }
}
