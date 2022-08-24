using Authentication.Contracts;
using Authentication.Presentation.Models;
using Authentication.Services.Abstraction;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public AuthController(IServiceManager serviceManager) => _serviceManager = serviceManager;

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        var userDto = request.Adapt<UserDto>();
        await _serviceManager.AuthenticationService.RegisterUserAsync(userDto);

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

        var tokenDto = request.Adapt<TokenDto>();
        TokenDto newTokenDto = await _serviceManager.AuthenticationService.RefreshTokenAsync(tokenDto);

        return Ok(newTokenDto);
    }
}
