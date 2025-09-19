using FluentResults;

namespace KitchenStock.Application.Modules.Auth.Results;

public static class AuthErrors
{
    public static Error InvalidCredentials =>
        new Error("Invalid email or password")
            .WithMetadata("ErrorCode", "AUTH_INVALID_CREDENTIALS")
            .WithMetadata("Type", "Authentication");

    public static Error UserNotFound(string email) =>
        new Error($"User with email '{email}' not found")
            .WithMetadata("ErrorCode", "AUTH_USER_NOT_FOUND")
            .WithMetadata("Email", email)
            .WithMetadata("Type", "NotFound");

    public static Error UserDeactivated =>
        new Error("User account is deactivated")
            .WithMetadata("ErrorCode", "AUTH_USER_DEACTIVATED")
            .WithMetadata("Type", "Authentication");

    public static Error TokenGenerationFailed =>
        new Error("Failed to generate authentication token")
            .WithMetadata("ErrorCode", "AUTH_TOKEN_GENERATION_FAILED")
            .WithMetadata("Type", "Internal");

    public static Error InvalidRefreshToken =>
        new Error("Invalid or expired refresh token")
            .WithMetadata("ErrorCode", "AUTH_INVALID_REFRESH_TOKEN")
            .WithMetadata("Type", "Authentication");

    public static Error RequiredFieldsMissing =>
        new Error("Email and password are required")
            .WithMetadata("ErrorCode", "AUTH_REQUIRED_FIELDS_MISSING")
            .WithMetadata("Type", "Validation");

    public static Error RefreshTokenExpired =>
        new Error("Refresh token has expired")
            .WithMetadata("ErrorCode", "AUTH_REFRESH_TOKEN_EXPIRED")
            .WithMetadata("Type", "Authentication");

    public static Error RefreshTokenRevoked =>
        new Error("Refresh token has been revoked")
            .WithMetadata("ErrorCode", "AUTH_REFRESH_TOKEN_REVOKED")
            .WithMetadata("Type", "Authentication");

    public static Error RefreshTokenNotFound =>
        new Error("Refresh token not found")
            .WithMetadata("ErrorCode", "AUTH_REFRESH_TOKEN_NOT_FOUND")
            .WithMetadata("Type", "Authentication");
}
