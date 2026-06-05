using ConsultationApi.Application.DTOs.Auth;
using ConsultationApi.Application.DTOs.Common;
using ConsultationApi.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultationApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(
        IAuthService authService)
    {
        _authService = authService;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);

        if (!result.Success)
            return BadRequest(result);

        // Return 201 Created with Location header pointing to the user resource (by email)
        var location = $"/api/users/{Uri.EscapeDataString(request.Email)}";

        return Created(location, result);
    }

    // POST: api/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request)
    {
        var result =
            await _authService.LoginAsync(request);

        return Ok(result);
    }

    // POST: api/auth/refresh
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequestDto request)
    {
        var result =
            await _authService.RefreshTokenAsync(
                request.RefreshToken);

        return Ok(result);
    }

    // GET: api/auth/me
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), 200)]
    public async Task<IActionResult> Me()
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ??
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var result =
            await _authService.GetCurrentUserAsync(userId);

        return Ok(result);
    }
}