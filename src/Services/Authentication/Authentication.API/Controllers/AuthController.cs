using Authentication.API.IntegrationEvents.Events;
using Authentication.API.Models;
using Authentication.Core.Contracts;
using Authentication.Core.Models;
using EventBus.Core.Abstractions;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    private readonly IEventBus _eventBus;


    public AuthController(IServiceManager serviceManager, IEventBus eventBus)
    {
        _serviceManager = serviceManager;
        _eventBus = eventBus;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        var user = request.Adapt<User>();
        Guid newId = await _serviceManager.AuthenticationService.RegisterUserAsync(user, request.Password);

        UserRegisteredSuccessfulIntegrationEvent @event = new(newId, user.Email, user.Email);
        _eventBus.Publish(@event);

        return Ok(newId);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Token))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        var user = request.Adapt<User>();
        Token token = await _serviceManager.AuthenticationService.LoginUserAsync(user, request.Password);

        return Ok(token);
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Token))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        Token oldToken = new(request.Token, request.RefreshToken, DateTime.UtcNow);
        Token newToken = await _serviceManager.AuthenticationService.RefreshTokenAsync(oldToken);

        return Ok(newToken);
    }
}
