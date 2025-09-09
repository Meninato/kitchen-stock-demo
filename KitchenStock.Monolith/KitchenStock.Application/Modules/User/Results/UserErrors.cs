using FluentResults;

namespace KitchenStock.Application.Modules.User.Results;

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
    }

    public static class BusinessRules
    {
        public static Error CannotCreateMoreKitchens(int maxAllowed, int current) =>
            new Error($"Cannot create more kitchens. Maximum allowed: {maxAllowed}, current: {current}")
                .WithMetadata("ErrorCode", "USER_MAX_KITCHENS_EXCEEDED")
                .WithMetadata("MaxAllowed", maxAllowed.ToString())
                .WithMetadata("Current", current.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error CannotDowngradePlan(string currentPlan, string newPlan, string reason) =>
            new Error($"Cannot downgrade from {currentPlan} to {newPlan}: {reason}")
                .WithMetadata("ErrorCode", "USER_CANNOT_DOWNGRADE_PLAN")
                .WithMetadata("CurrentPlan", currentPlan)
                .WithMetadata("NewPlan", newPlan)
                .WithMetadata("Type", "BusinessRule");

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

    public static Error UserNotFound(string email) =>
        new Error($"User with email {email} not found")
            .WithMetadata("ErrorCode", "USER_NOT_FOUND")
            .WithMetadata("Email", email)
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
