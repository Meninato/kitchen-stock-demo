using MediatR;
using Microsoft.AspNetCore.Mvc;
using User.Application.Commands;

namespace User.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Authenticate user and get JWT token
    /// </summary>
    /// <param name="command">Login credentials</param>
    /// <returns>JWT authentication token</returns>
    /// <response code="200">Authentication successful</response>
    /// <response code="401">Invalid credentials or user deactivated</response>
    /// <response code="404">User not found</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        throw new Exception("FORCING EX");
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(new
            {
                data = new { token = result.Value },
            });
        }

        return result.Errors.FirstToActionResult(this);
    }
}
