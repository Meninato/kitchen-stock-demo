using FluentResults;

namespace KitchenStock.Application.Errors;

public static class UserErrors
{
    public static class Registration
    {
        public static Error EmailAlreadyExists(string email) =>
            new Error($"Email '{email}' is already registered")
                .WithMetadata("ErrorCode", "USER_EMAIL_EXISTS")
                .WithMetadata("Field", "Email")
                .WithMetadata("Type", "Conflict");

        public static Error InvalidEmail(string email) =>
            new Error($"Email '{email}' has invalid format")
                .WithMetadata("ErrorCode", "USER_INVALID_EMAIL")
                .WithMetadata("Field", "Email")
                .WithMetadata("Type", "Validation");

        public static Error PasswordTooWeak =>
            new Error("Password must contain at least 6 characters, including uppercase, lowercase and numbers")
                .WithMetadata("ErrorCode", "USER_WEAK_PASSWORD")
                .WithMetadata("Field", "Password")
                .WithMetadata("Type", "Validation");

        public static Error NameRequired =>
            new Error("Name is required")
                .WithMetadata("ErrorCode", "USER_NAME_REQUIRED")
                .WithMetadata("Field", "Name")
                .WithMetadata("Type", "Validation");

        public static Error NameTooLong(int maxLength) =>
            new Error($"Name must not exceed {maxLength} characters")
                .WithMetadata("ErrorCode", "USER_NAME_TOO_LONG")
                .WithMetadata("Field", "Name")
                .WithMetadata("Type", "Validation");

        public static Error InvalidPlanForEmail(string plan, string email) =>
            new Error($"Plan '{plan}' is not available for email '{email}'. Corporate email required.")
                .WithMetadata("ErrorCode", "USER_INVALID_PLAN")
                .WithMetadata("Field", "Plan")
                .WithMetadata("Type", "BusinessRule");
    }

    public static class Authentication
    {
        public static Error InvalidCredentials =>
            new Error("Invalid email or password")
                .WithMetadata("ErrorCode", "USER_INVALID_CREDENTIALS")
                .WithMetadata("Type", "Authentication");

        public static Error UserNotFound(string email) =>
            new Error($"User with email '{email}' not found")
                .WithMetadata("ErrorCode", "USER_NOT_FOUND")
                .WithMetadata("Type", "NotFound");

        public static Error TokenGenerationFailed =>
            new Error("Failed to generate authentication token")
                .WithMetadata("ErrorCode", "USER_TOKEN_FAILED")
                .WithMetadata("Type", "Internal");
    }

    public static class BusinessRules
    {
        public static Error PlanNotAvailable(string plan) =>
            new Error($"Plan '{plan}' is not available")
                .WithMetadata("ErrorCode", "USER_PLAN_NOT_AVAILABLE")
                .WithMetadata("Plan", plan)
                .WithMetadata("Type", "BusinessRule");
    }

    public static Error UserNotFound(Guid userId) =>
        new Error($"User with ID {userId} not found")
            .WithMetadata("ErrorCode", "USER_NOT_FOUND")
            .WithMetadata("UserId", userId.ToString())
            .WithMetadata("Type", "NotFound");

    public static Error DatabaseError(string operation, Exception? ex = null) =>
        new Error($"Database error during {operation}")
            .WithMetadata("ErrorCode", "USER_DATABASE_ERROR")
            .WithMetadata("Operation", operation)
            .WithMetadata("Type", "Internal")
            .CausedBy(ex);

    public static Error UnexpectedError(string operation, Exception? ex = null) =>
        new Error($"Unexpected error during {operation}")
            .WithMetadata("ErrorCode", "USER_UNEXPECTED_ERROR")
            .WithMetadata("Operation", operation)
            .WithMetadata("Type", "Internal")
            .CausedBy(ex);
}
