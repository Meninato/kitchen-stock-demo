using KitchenStock.Api.Extensions;
using KitchenStock.Application.Errors;
using KitchenStock.Application.User.Commands;
using KitchenStock.Application.User.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

    /// <summary>
    /// Get user profile by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User profile information</returns>
    /// <response code="200">User found</response>
    /// <response code="403">Access forbidden</response>
    /// <response code="404">User not found</response>
    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 403)]
    [ProducesResponseType(typeof(object), 404)]
    public async Task<IActionResult> GetProfile(Guid id)
    {
        var currentUserId = User.FindFirst("sub")?.Value ?? string.Empty;
        if (currentUserId != id.ToString())
            return UserErrors.AccessDenied().ToActionResult(this);

        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(new
            {
                data = result.Value
            });
        }

        return result.Errors.FirstToActionResult(this);
    }
}