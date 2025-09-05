using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace User.Api;

public static class ErrorMap
{
    public static IActionResult FirstToActionResult(this IReadOnlyList<IError> errors, ControllerBase controller)
    {
        var error = errors.First();
        return error.ToActionResult(controller);
    }

    public static IActionResult ToActionResult(this IError error, ControllerBase controller)
    {
        var data = new { error.Message, error.Metadata };

        return error.Metadata.TryGetValue("Type", out var type) ? type switch
        {
            "Conflict" => controller.Conflict(data),
            "Validation" => controller.BadRequest(data),
            "BusinessRule" => controller.BadRequest(data),
            "Authentication" => controller.Unauthorized(data),
            "NotFound" => controller.NotFound(data),
            _ => controller.Problem(error.Message)
        } : controller.Problem(error.Message);
    }
}
