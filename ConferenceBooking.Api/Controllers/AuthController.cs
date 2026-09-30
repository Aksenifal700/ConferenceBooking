using ConferenceBooking.Application.Interfaces.Services;
using ConferenceBooking.Mapping;
using ConferenceBooking.Models.Requests.Auth;
using ConferenceBooking.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBooking.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var userId = await _authService.RegisterAsync(request.ToDto());

        return Ok(userId);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthTokenResponse>> Login(
        [FromBody] LoginRequest request)
    {
        var token = await _authService.LoginAsync(request.ToDto());

        return Ok(token.ToResponse());
    }
}