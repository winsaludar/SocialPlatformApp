using Chat.API.Models;
using Chat.Application.Commands;
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

    public ServersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> PostAsync([FromBody] CreateServerRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        var result = await _mediator.Send(new CreateServerCommand(request.Name, request.ShortDescription, request.LongDescription, request.Thumbnail));

        return Ok(result);
    }
}
