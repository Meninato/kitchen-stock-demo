using KitchenStock.Application.Modules.User.Dtos;
using KitchenStock.Application.Modules.User.MediatR.Commands;
using KitchenStock.Application.Modules.User.MediatR.Queries;
using KitchenStock.Application.Modules.User.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenStock.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
    {
        //TODO: review user API to see if is necessary to let all of this in the endpoint unless you have an admin
        //role maybe

        return ErrorToActionResult(UserErrors.Authorization.AccessDenied);

        //always register the user as basic plan
        var command = new CreateUserCommand(request.Name, request.Email, request.Password);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return ApiCreatedAtAction(
                nameof(GetUser),
                "Users",
                new { id = result.Value.Id },
                result.Value
            );
        }

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        if (CurrentUserId != id)
            return ErrorToActionResult(UserErrors.Authorization.AccessDenied);

        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return ApiOk(result.Value);

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto request)
    {
        if (CurrentUserId != id)
            return ErrorToActionResult(UserErrors.Authorization.AccessDenied);

        var command = new UpdateUserCommand(id, request.Name, request.Email);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return ApiOk(result.Value);

        return FirstErrorToActionResult(result.Errors);
    }

    [HttpPut("{id}/plan")]
    public async Task<IActionResult> UpdatePlan(Guid id, [FromBody] UpdateUserPlanDto request)
    {
        if (CurrentUserId != id)
            return ErrorToActionResult(UserErrors.Authorization.AccessDenied);

        var command = new UpdateUserPlanCommand(id, request.Plan);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return ApiOk(result.Value);

        return FirstErrorToActionResult(result.Errors);
    }
}