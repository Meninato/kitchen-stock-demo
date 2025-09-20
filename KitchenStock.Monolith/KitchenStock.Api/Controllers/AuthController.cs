using KitchenStock.Application.Modules.Auth.Constants;
using KitchenStock.Application.Modules.Auth.Dtos;
using KitchenStock.Application.Modules.Auth.MediatR.Commands;
using KitchenStock.Application.Modules.Auth.Results;
using KitchenStock.Application.Modules.User.Dtos;
using KitchenStock.Application.Modules.User.MediatR.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateUserDto request)
    {
        var ipAddress = GetIpAddress();
        var userAgent = GetUserAgent();

        var command = new AuthenticateUserCommand(request.Email, request.Password, ipAddress, userAgent);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            SetTokenCookies(result.Value.AccessToken, result.Value.RefreshToken);
            return Ok();
        }

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
    {
        //always register the user as basic plan
        var command = new CreateUserCommand(request.Name, request.Email, request.Password);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return ApiCreatedAtAction(
                nameof(UsersController.GetUser),
                "Users",
                new { id = result.Value.Id },
                result.Value
            );
        }

        return FirstErrorToActionResult(result.Errors);
    }

    /// <summary>
    /// Refresh JWT token using HttpOnly refresh token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies[AuthCookieNames.RefreshToken];
        if (string.IsNullOrEmpty(refreshToken))
            return ErrorToActionResult(AuthErrors.MissingRefreshToken);

        var ipAddress = GetIpAddress();
        var userAgent = GetUserAgent();

        var command = new RefreshTokenCommand(refreshToken, ipAddress, userAgent);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            SetTokenCookies(result.Value.AccessToken, result.Value.RefreshToken);
            return Ok();
        }

        // Clear cookies on failed refresh
        ClearTokenCookies();
        return FirstErrorToActionResult(result.Errors);
    }

    /// <summary>
    /// Logout and revoke refresh token
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies[AuthCookieNames.RefreshToken];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            var command = new LogoutCommand(refreshToken);
            await _mediator.Send(command);
        }

        ClearTokenCookies();

        return Ok();
    }

    /// <summary>
    /// Revoke all user tokens (security endpoint)
    /// </summary>
    [Authorize]
    [HttpPost("revoke-all")]
    public async Task<IActionResult> RevokeAllTokens()
    {
        var command = new RevokeAllTokensCommand(CurrentUserId, "User requested token revocation");
        var result = await _mediator.Send(command);

        ClearTokenCookies();

        if (result.IsSuccess)
            return Ok();

       return ErrorToActionResult(AuthErrors.RevokeAllRefreshTokenFailed);
    }

    private void SetTokenCookies(AccessTokenDto accessToken, RefreshTokenDto refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !HttpContext.Request.Host.Host.Contains("localhost"), // HTTPS in production
            SameSite = SameSiteMode.Strict,
            Path = "/"
        };

        // Access token cookie (shorter expiration)
        var accessCookieOptions = cookieOptions;
        accessCookieOptions.Expires = accessToken.UtcExpiresAt;
        Response.Cookies.Append(AuthCookieNames.AccessToken, accessToken.TokenValue, accessCookieOptions);

        // Refresh token cookie (longer expiration)
        var refreshCookieOptions = cookieOptions;
        refreshCookieOptions.Expires = refreshToken.UtcExpiresAt;
        Response.Cookies.Append(AuthCookieNames.RefreshToken, refreshToken.TokenValue, refreshCookieOptions);
    }

    private void ClearTokenCookies()
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !HttpContext.Request.Host.Host.Contains("localhost"),
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = DateTime.UtcNow.AddDays(-1) // Expire immediately
        };

        Response.Cookies.Append(AuthCookieNames.AccessToken, "", cookieOptions);
        Response.Cookies.Append(AuthCookieNames.RefreshToken, "", cookieOptions);
    }

    private string GetIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private string GetUserAgent()
    {
        return HttpContext.Request.Headers["User-Agent"].ToString() ?? "unknown";
    }
}
