using FluentResults;

namespace KitchenStock.Application.Errors;

public static class AuthErrors
{
    public static class Authentication
    {
        public static Error InvalidCredentials =>
            new Error("Invalid email or password")
                .WithMetadata("ErrorCode", "AUTH_INVALID_CREDENTIALS")
                .WithMetadata("Type", "Authentication");

        public static Error UserNotFound(string email) =>
            new Error($"User with email '{email}' not found")
                .WithMetadata("ErrorCode", "AUTH_USER_NOT_FOUND")
                .WithMetadata("Type", "NotFound");

        public static Error TokenGenerationFailed =>
            new Error("Failed to generate authentication token")
                .WithMetadata("ErrorCode", "AUTH_TOKEN_FAILED")
                .WithMetadata("Type", "Internal");
    }

    public static Error UnexpectedError(string operation, Exception? ex = null) =>
        new Error($"Unexpected error during {operation}")
            .WithMetadata("ErrorCode", "AUTH_UNEXPECTED_ERROR")
            .WithMetadata("Operation", operation)
            .WithMetadata("Type", "Internal")
            .CausedBy(ex);
}
