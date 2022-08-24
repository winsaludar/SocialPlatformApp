using Authentication.Contracts;
using Authentication.Domain.Entities;
using Authentication.Domain.Repositories;
using Authentication.Presentation.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IRepositoryManager _repositoryManager;

    public AuthController(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        User newUser = new()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
        };
        await newUser.RegisterAsync(request.Password, _repositoryManager);

        return Ok("User created");
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        User user = new() { Email = request.Email };
        Token token = await user.LoginAsync(request.Password, _repositoryManager);

        var dto = token.Adapt<TokenDto>();
        return Ok(dto);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Please provide all the required fields");

        Token token = new() { Value = request.Token, RefreshToken = request.RefreshToken };
        await token.RefreshAsync(_repositoryManager);

        var dto = token.Adapt<TokenDto>();
        return Ok(dto);
    }
}
