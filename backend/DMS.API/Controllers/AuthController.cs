using System.Security.Claims;
using DMS.Application.DTOs;
using DMS.Application.Services;
using DMS.Core.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var (success, data, errors) = await _authService.RegisterAsync(dto);
        if (!success)
            return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse("Kayıt başarısız.", errors));

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(data!, "Kayıt başarılı."));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var (success, data, error) = await _authService.LoginAsync(dto);
        if (!success)
            return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse(error!));

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(data!, "Giriş başarılı."));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var user = await _authService.GetCurrentUserAsync(userId);
        if (user == null) return NotFound();

        return Ok(ApiResponse<UserDto>.SuccessResponse(user));
    }
}
