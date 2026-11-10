using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UserDirectory.Api.DTOs;
using UserDirectory.Api.Services;

namespace UserDirectory.Api.Controllers;

[ApiController]
[EnableRateLimiting("AuthEndpoints")]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<LocalAuthResponse>> Register(
        LocalAuthRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _authService.RegisterAsync(request, cancellationToken));
        }
        catch (DuplicateAccountException)
        {
            return Conflict(new { message = "An account with this email already exists." });
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LocalAuthResponse>> Login(
        LocalAuthRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request, cancellationToken);
        if (response is null)
        {
            return Unauthorized(new { message = "Email or password is incorrect." });
        }

        return Ok(response);
    }
}
