using Authentication.API.IntegrationEvents.Events;
using Authentication.API.Models;
using Authentication.Contracts;
using Authentication.Services.Abstraction;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        var userDto = request.Adapt<UserDto>();
        Guid newId = await _serviceManager.AuthenticationService.RegisterUserAsync(userDto);

        UserRegisteredSuccessfulIntegrationEvent @event = new(newId, userDto.Email, userDto.Email);
        _eventBus.Publish(@event);

        return Ok("User created");
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        var userDto = request.Adapt<UserDto>();
        TokenDto tokenDto = await _serviceManager.AuthenticationService.LoginUserAsync(userDto);

        return Ok(tokenDto);
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        TokenDto dto = new() { Value = request.Token, RefreshToken = request.RefreshToken };
        TokenDto newTokenDto = await _serviceManager.AuthenticationService.RefreshTokenAsync(dto);

        return Ok(newTokenDto);
    }
}
