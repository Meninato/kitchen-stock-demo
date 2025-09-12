using FluentResults;
using KitchenStock.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace KitchenStock.Api.Controllers;

public class ApiControllerBase : ControllerBase
{
    protected Guid CurrentUserId
    {
        get
        {
            var claim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (Guid.TryParse(claim, out var userId))
                return userId;

            throw new UnauthorizedAccessException("Invalid or missing user id in token.");
        }
    }

    protected IActionResult ApiOk<T>(Result<T> result)
    {
        return Ok(new ApiResponse<T>(result.Value));
    }

    protected IActionResult ApiCreatedAtAction<T>(string actionName, string controllerName, object? routeValues, T data)
    {
        return CreatedAtAction(actionName, controllerName, routeValues, new ApiResponse<T>(data));
    }

    protected IActionResult FirstErrorToActionResult(IReadOnlyList<IError> errors)
    {
        var error = errors.First();
        return ErrorToActionResult(error);
    }

    protected IActionResult ErrorToActionResult(IError error)
    {
        var data = new { error.Message, error.Metadata };

        return error.Metadata.TryGetValue("Type", out var type) ? type switch
        {
            "Conflict" => Conflict(data),
            "Validation" => BadRequest(data),
            "BusinessRule" => BadRequest(data),
            "Authentication" => Unauthorized(data),
            "NotFound" => NotFound(data),
            "Forbidden" => new ObjectResult(data) { StatusCode = StatusCodes.Status403Forbidden },
            _ => Problem(error.Message)
        } : Problem(error.Message);
    }
}
