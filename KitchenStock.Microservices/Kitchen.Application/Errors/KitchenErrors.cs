using FluentResults;

namespace Kitchen.Application.Errors;

public static class KitchenErrors
{
    public static class Creation
    {
        public static Error NameRequired =>
            new Error("Kitchen name is required")
                .WithMetadata("ErrorCode", "KITCHEN_NAME_REQUIRED")
                .WithMetadata("Field", "Name")
                .WithMetadata("Type", "Validation");

        public static Error NameTooLong(int maxLength) =>
            new Error($"Kitchen name must not exceed {maxLength} characters")
                .WithMetadata("ErrorCode", "KITCHEN_NAME_TOO_LONG")
                .WithMetadata("Field", "Name")
                .WithMetadata("Type", "Validation");

        public static Error MaxKitchensExceeded(int maxAllowed, int current) =>
            new Error($"Maximum kitchens exceeded. Allowed: {maxAllowed}, Current: {current}")
                .WithMetadata("ErrorCode", "KITCHEN_MAX_EXCEEDED")
                .WithMetadata("MaxAllowed", maxAllowed.ToString())
                .WithMetadata("Current", current.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error UserNotFound(Guid userId) =>
            new Error($"User with ID {userId} not found")
                .WithMetadata("ErrorCode", "KITCHEN_USER_NOT_FOUND")
                .WithMetadata("UserId", userId.ToString())
                .WithMetadata("Type", "NotFound");

        public static Error SharedIngredientsNotAllowed =>
            new Error("Shared ingredients feature requires Premium plan")
                .WithMetadata("ErrorCode", "KITCHEN_SHARED_INGREDIENTS_PREMIUM")
                .WithMetadata("Feature", "SharedIngredients")
                .WithMetadata("Type", "BusinessRule");
    }

    public static class Access
    {
        public static Error KitchenNotFound(Guid kitchenId) =>
            new Error($"Kitchen with ID {kitchenId} not found")
                .WithMetadata("ErrorCode", "KITCHEN_NOT_FOUND")
                .WithMetadata("KitchenId", kitchenId.ToString())
                .WithMetadata("Type", "NotFound");

        public static Error AccessDenied(Guid userId, Guid kitchenId) =>
            new Error($"User {userId} does not have access to kitchen {kitchenId}")
                .WithMetadata("ErrorCode", "KITCHEN_ACCESS_DENIED")
                .WithMetadata("UserId", userId.ToString())
                .WithMetadata("KitchenId", kitchenId.ToString())
                .WithMetadata("Type", "Forbidden");

        public static Error KitchenInactive(Guid kitchenId) =>
            new Error($"Kitchen {kitchenId} is inactive")
                .WithMetadata("ErrorCode", "KITCHEN_INACTIVE")
                .WithMetadata("KitchenId", kitchenId.ToString())
                .WithMetadata("Type", "BusinessRule");
    }

    public static class UserApi
    {
        public static Error UserApiUnavailable =>
            new Error("User API is currently unavailable")
                .WithMetadata("ErrorCode", "KITCHEN_USER_API_UNAVAILABLE")
                .WithMetadata("Type", "ServiceUnavailable");

        public static Error UserValidationFailed(string reason) =>
            new Error($"User validation failed: {reason}")
                .WithMetadata("ErrorCode", "KITCHEN_USER_VALIDATION_FAILED")
                .WithMetadata("Reason", reason)
                .WithMetadata("Type", "BusinessRule");
    }

    public static Error DatabaseError(string operation, Exception? ex = null) =>
        new Error($"Database error during {operation}")
            .WithMetadata("ErrorCode", "KITCHEN_DATABASE_ERROR")
            .WithMetadata("Operation", operation)
            .WithMetadata("Type", "Internal")
            .CausedBy(ex);

    public static Error UnexpectedError(string operation, Exception? ex = null) =>
        new Error($"Unexpected error during {operation}")
            .WithMetadata("ErrorCode", "KITCHEN_UNEXPECTED_ERROR")
            .WithMetadata("Operation", operation)
            .WithMetadata("Type", "Internal")
            .CausedBy(ex);
}
