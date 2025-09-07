using KitchenStock.Application.User.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KitchenStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="command">User registration data</param>
    /// <returns>Created user information</returns>
    /// <response code="201">User created successfully</response>
    /// <response code="400">Validation errors or business rule violations</response>
    /// <response code="409">Email already exists</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(object), 201)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 409)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetProfile),
                "Users",
                new { id = result.Value.Id },
                new { data = result.Value }
            );
        }

        return result.Errors.FirstToActionResult(this);
    }
}