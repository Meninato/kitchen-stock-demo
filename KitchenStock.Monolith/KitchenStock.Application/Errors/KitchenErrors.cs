using FluentResults;

namespace KitchenStock.Application.Errors;

public static class KitchenErrors
{
    public static class Validation
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

        public static Error DescriptionTooLong(int maxLength) =>
            new Error($"Kitchen description must not exceed {maxLength} characters")
                .WithMetadata("ErrorCode", "KITCHEN_DESCRIPTION_TOO_LONG")
                .WithMetadata("Field", "Description")
                .WithMetadata("Type", "Validation");

        public static Error InvalidKitchenType(string type) =>
            new Error($"Kitchen type '{type}' is not valid")
                .WithMetadata("ErrorCode", "KITCHEN_INVALID_TYPE")
                .WithMetadata("Field", "Type")
                .WithMetadata("Type", "Validation");
    }

    public static class BusinessRules
    {
        public static Error MaxKitchensReached(int maxAllowed, int current) =>
            new Error($"Cannot create more kitchens. Maximum allowed: {maxAllowed}, current: {current}")
                .WithMetadata("ErrorCode", "KITCHEN_MAX_LIMIT_REACHED")
                .WithMetadata("MaxAllowed", maxAllowed.ToString())
                .WithMetadata("Current", current.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error CannotDeleteWithActiveIngredients(int ingredientCount) =>
            new Error($"Cannot delete kitchen with {ingredientCount} active ingredients. Remove ingredients first.")
                .WithMetadata("ErrorCode", "KITCHEN_HAS_ACTIVE_INGREDIENTS")
                .WithMetadata("IngredientCount", ingredientCount.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error CannotDeleteWithActiveRecipes(int recipeCount) =>
            new Error($"Cannot delete kitchen with {recipeCount} active recipes. Remove recipes first.")
                .WithMetadata("ErrorCode", "KITCHEN_HAS_ACTIVE_RECIPES")
                .WithMetadata("RecipeCount", recipeCount.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error KitchenNameAlreadyExists(string name, Guid userId) =>
            new Error($"You already have a kitchen named '{name}'")
                .WithMetadata("ErrorCode", "KITCHEN_NAME_DUPLICATE")
                .WithMetadata("Field", "Name")
                .WithMetadata("UserId", userId.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error UserPlanDoesNotAllowMultipleKitchens(string currentPlan) =>
            new Error($"Your current plan ({currentPlan}) allows only one kitchen. Upgrade to Premium for multiple kitchens.")
                .WithMetadata("ErrorCode", "KITCHEN_PLAN_LIMITATION")
                .WithMetadata("CurrentPlan", currentPlan)
                .WithMetadata("Type", "BusinessRule");
    }

    public static class Authorization
    {
        public static Error NotFound(Guid kitchenId) =>
            new Error($"Kitchen with ID {kitchenId} not found")
                .WithMetadata("ErrorCode", "KITCHEN_NOT_FOUND")
                .WithMetadata("KitchenId", kitchenId.ToString())
                .WithMetadata("Type", "NotFound");

        public static Error AccessDenied(Guid kitchenId, Guid userId) =>
            new Error($"User does not have access to kitchen {kitchenId}")
                .WithMetadata("ErrorCode", "KITCHEN_ACCESS_DENIED")
                .WithMetadata("KitchenId", kitchenId.ToString())
                .WithMetadata("UserId", userId.ToString())
                .WithMetadata("Type", "Forbidden");

        public static Error NotOwner(Guid kitchenId, int userId) =>
            new Error($"User is not the owner of kitchen {kitchenId}")
                .WithMetadata("ErrorCode", "KITCHEN_NOT_OWNER")
                .WithMetadata("KitchenId", kitchenId.ToString())
                .WithMetadata("UserId", userId.ToString())
                .WithMetadata("Type", "Forbidden");
    }

    public static class Operations
    {
        public static Error CalculationFailed(string operation, string reason) =>
            new Error($"Failed to calculate {operation}: {reason}")
                .WithMetadata("ErrorCode", "KITCHEN_CALCULATION_FAILED")
                .WithMetadata("Operation", operation)
                .WithMetadata("Reason", reason)
                .WithMetadata("Type", "Internal");

        public static Error StatsCalculationFailed(Guid kitchenId, Exception? ex = null) =>
            new Error($"Failed to calculate statistics for kitchen {kitchenId}")
                .WithMetadata("ErrorCode", "KITCHEN_STATS_CALCULATION_FAILED")
                .WithMetadata("KitchenId", kitchenId.ToString())
                .WithMetadata("Type", "Internal")
                .CausedBy(ex);
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
