using KitchenStock.Application.Auth.Commands;
using MediatR;
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthenticateUserCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(result);

        return FirstErrorToActionResult(result.Errors);
    }
}
