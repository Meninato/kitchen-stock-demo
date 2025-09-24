using FluentResults;
using KitchenStock.Api.Common;
using KitchenStock.Api.Common.Pagination;
using KitchenStock.Api.Helpers;
using KitchenStock.Application.Common.Pagination.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace KitchenStock.Api.Controllers;

public class ApiControllerBase : ControllerBase
{
    protected Guid CurrentUserId
    {
        get
        {
            var claim =
                User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(claim, out var userId))
                return userId;

            throw new UnauthorizedAccessException("Invalid or missing user id in token.");
        }
    }

    protected IActionResult ApiOk<T>(T data, Action<ApiResponseBuilder<T>>? configure = null)
    {
        var builder = new ApiResponseBuilder<T>(data);
        configure?.Invoke(builder);

        return Ok(builder.Build());
    }

    protected IActionResult ApiCreatedAtAction<T>(string actionName, string controllerName, object? routeValues, T data, Action<ApiResponseBuilder<T>>? configure = null)
    {
        var builder = new ApiResponseBuilder<T>(data);
        configure?.Invoke(builder);

        return CreatedAtAction(actionName, controllerName, routeValues, builder.Build());
    }

    protected IActionResult FirstErrorToActionResult(IReadOnlyList<IError> errors)
    {
        var error = errors.First();
        return ErrorToActionResult(error);
    }

    protected IActionResult ErrorToActionResult(IError error)
    {
        var data = new { message = error.Message, metadata = error.Metadata.ToCamelCaseKeys() };

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
