using Chat.API.Extensions;
using Chat.Application.Commands;
using Chat.Application.DTOs;
using Chat.Application.Queries;
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
    private readonly IValidator<CreateServerCommand> _createServerValidator;

    public ServersController(IMediator mediator, IValidator<CreateServerCommand> createServerValidator)
    {
        _mediator = mediator;
        _createServerValidator = createServerValidator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ServerDto>))]
    public async Task<IActionResult> GetAsync()
    {
        var result = await _mediator.Send(new GetServersQuery());
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> PostAsync([FromBody] CreateServerCommand command)
    {
        ValidationResult validationResult = await _createServerValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        var result = await _mediator.Send(command);

        return Ok(result.ToString());
    }
}
