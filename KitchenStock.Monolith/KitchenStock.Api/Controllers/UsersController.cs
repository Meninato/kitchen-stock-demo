using KitchenStock.Application.Errors;
using KitchenStock.Application.User.Commands;
using KitchenStock.Application.User.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return ApiCreatedAtAction(
                nameof(GetProfile),
                "Users",
                new { id = result.Value.Id },
                result.Value
            );
        }

        return FirstErrorToActionResult(result.Errors);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfile(Guid id)
    {
        if (CurrentUserId != id)
            return ErrorToActionResult(UserErrors.AccessDenied());

        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result);

        return FirstErrorToActionResult(result.Errors);
    }
}