using Authentication.Contracts;
using Authentication.Presentation.Models;
using Authentication.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public AuthController(IServiceManager serviceManager) => _serviceManager = serviceManager;

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterApplicationUser user)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        RegisterApplicationUserDto newUser = new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Password = user.Password
        };
        await _serviceManager.ApplicationUserService.RegisterAsync(newUser);

        return Ok("User created");
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginApplicationUser user)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        LoginUserDto login = new() { Email = user.Email, Password = user.Password };
        var token = await _serviceManager.ApplicationUserService.LoginAsync(login);

        return Ok(token);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] TokenRequest token)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        TokenDto oldToken = new() { Token = token.Token, RefreshToken = token.RefreshToken };
        var newToken = await _serviceManager.ApplicationUserService.GenerateNewTokenAsync(oldToken);

        return Ok(newToken);
    }
}
