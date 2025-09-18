using FluentResults;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace KitchenStock.Api.Helpers;

public class InvalidModelStateResponse
{
    public string Message => "One or more validation errors occurred.";
    public List<Error> Errors { get; set; } = new List<Error>();
    public bool IsSuccess => !Errors.Any();

    public InvalidModelStateResponse(ModelStateDictionary modelState)
    {
        var errorsInModelState = modelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage)
            .ToArray());

        foreach (var error in errorsInModelState)
        {
            if (error.Value != null)
            {
                foreach (var subError in error.Value)
                {
                    var fluentError = new Error(subError)
                        .WithMetadata("error_code", GenerateErrorCode(error.Key, subError))
                        .WithMetadata("field", error.Key)
                        .WithMetadata("type", "Validation");

                    Errors.Add(fluentError);
                }
            }
        }
    }

    private static string GenerateErrorCode(string fieldName, string errorMessage)
    {
        // Generate consistent error codes based on field name and common validation patterns
        var field = fieldName.ToUpper().Replace(".", "_");

        if (errorMessage.Contains("required", StringComparison.OrdinalIgnoreCase))
            return $"{field}_REQUIRED";
        if (errorMessage.Contains("invalid", StringComparison.OrdinalIgnoreCase))
            return $"{field}_INVALID";
        if (errorMessage.Contains("length", StringComparison.OrdinalIgnoreCase))
            return $"{field}_LENGTH_INVALID";
        if (errorMessage.Contains("format", StringComparison.OrdinalIgnoreCase))
            return $"{field}_FORMAT_INVALID";

        return $"{field}_VALIDATION_ERROR";
    }
}
