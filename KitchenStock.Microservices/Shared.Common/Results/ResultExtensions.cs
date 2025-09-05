using FluentResults;

namespace Shared.Common.Results;

public static class ResultExtensions
{
    public static string GetErrorCode(this IError error)
    {
        return error.GetMetadataValue("ErrorCode", "UNKNOWN_ERROR");
    }

    public static string GetErrorType(this IError error)
    {
        return error.GetMetadataValue("Type", "Unknown");
    }

    public static string GetField(this IError error)
    {
        return error.GetMetadataValue("Field", string.Empty);
    }

    public static Dictionary<string, object> GetErrorDetails(this IError error)
    {
        return new Dictionary<string, object>
        {
            ["code"] = error.GetErrorCode(),
            ["message"] = error.Message,
            ["type"] = error.GetErrorType(),
            ["field"] = error.GetField(),
            ["metadata"] = error.Metadata
        };
    }

    public static List<Dictionary<string, object>> GetErrorDetails(this IEnumerable<IError> errors)
    {
        return errors.Select(e => e.GetErrorDetails()).ToList();
    }

    private static string GetMetadataValue(this IError error, string key, string fallback = "")
    {
        return error.Metadata.TryGetValue(key, out var value) && value is not null
            ? value.ToString()!
            : fallback;
    }
}
