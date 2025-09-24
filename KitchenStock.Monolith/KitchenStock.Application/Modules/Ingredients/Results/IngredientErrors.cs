using FluentResults;

namespace KitchenStock.Application.Modules.Ingredients.Results;

public static class IngredientErrors
{
    public static class Validation
    {
        public static Error NameRequired =>
            new Error("Ingredient name is required")
                .WithMetadata("ErrorCode", "INGREDIENT_NAME_REQUIRED")
                .WithMetadata("Field", "Name")
                .WithMetadata("Type", "Validation");

        public static Error NameTooLong(int maxLength) =>
            new Error($"Ingredient name must not exceed {maxLength} characters")
                .WithMetadata("ErrorCode", "INGREDIENT_NAME_TOO_LONG")
                .WithMetadata("Field", "Name")
                .WithMetadata("Type", "Validation");

        public static Error DescriptionTooLong(int maxLength) =>
            new Error($"Ingredient description must not exceed {maxLength} characters")
                .WithMetadata("ErrorCode", "INGREDIENT_DESCRIPTION_TOO_LONG")
                .WithMetadata("Field", "Description")
                .WithMetadata("Type", "Validation");

        public static Error CategoryTooLong(int maxLength) =>
            new Error($"Ingredient category must not exceed {maxLength} characters")
                .WithMetadata("ErrorCode", "INGREDIENT_CATEGORY_TOO_LONG")
                .WithMetadata("Field", "Category")
                .WithMetadata("Type", "Validation");

        public static Error InvalidMinimumStock =>
            new Error("Minimum stock must be greater than or equal to zero")
                .WithMetadata("ErrorCode", "INGREDIENT_INVALID_MINIMUM_STOCK")
                .WithMetadata("Field", "MinimumStock")
                .WithMetadata("Type", "Validation");

        public static Error InvalidCurrentStock =>
            new Error("Current stock must be greater than or equal to zero")
                .WithMetadata("ErrorCode", "INGREDIENT_INVALID_CURRENT_STOCK")
                .WithMetadata("Field", "CurrentStock")
                .WithMetadata("Type", "Validation");

        public static Error InvalidUnitOfMeasure(Guid unitId) =>
            new Error($"Unit of measure with ID {unitId} does not exist")
                .WithMetadata("ErrorCode", "INGREDIENT_INVALID_UNIT_OF_MEASURE")
                .WithMetadata("Field", "UnitOfMeasureId")
                .WithMetadata("UnitId", unitId.ToString())
                .WithMetadata("Type", "Validation");

        public static Error MissingKitchenId =>
            new Error($"Missing kitchen ID")
                .WithMetadata("ErrorCode", "INGREDIENT_MISSING_KITCHEN")
                .WithMetadata("Field", "KitchenId")
                .WithMetadata("Type", "Validation");
    }

    public static class BusinessRules
    {
        public static Error AlreadyExistsInKitchen(string name, Guid kitchenId) =>
            new Error($"Ingredient '{name}' already exists in this kitchen")
                .WithMetadata("ErrorCode", "INGREDIENT_ALREADY_EXISTS")
                .WithMetadata("Field", "Name")
                .WithMetadata("IngredientName", name)
                .WithMetadata("KitchenId", kitchenId.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error CannotDeleteWithActiveRecipes(string ingredientName, int recipeCount) =>
            new Error($"Cannot delete ingredient '{ingredientName}' used in {recipeCount} active recipes")
                .WithMetadata("ErrorCode", "INGREDIENT_USED_IN_RECIPES")
                .WithMetadata("IngredientName", ingredientName)
                .WithMetadata("RecipeCount", recipeCount.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error CannotDeleteWithStockHistory(string ingredientName, int entryCount) =>
            new Error($"Cannot delete ingredient '{ingredientName}' with {entryCount} stock history entries")
                .WithMetadata("ErrorCode", "INGREDIENT_HAS_STOCK_HISTORY")
                .WithMetadata("IngredientName", ingredientName)
                .WithMetadata("EntryCount", entryCount.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error InsufficientStock(string ingredientName, decimal requested, decimal available) =>
            new Error($"Insufficient stock for '{ingredientName}'. Requested: {requested}, Available: {available}")
                .WithMetadata("ErrorCode", "INGREDIENT_INSUFFICIENT_STOCK")
                .WithMetadata("IngredientName", ingredientName)
                .WithMetadata("RequestedQuantity", requested.ToString())
                .WithMetadata("AvailableQuantity", available.ToString())
                .WithMetadata("Type", "BusinessRule");
    }

    public static class Authorization
    {
        public static Error NotFound(Guid ingredientId) =>
            new Error($"Ingredient with ID {ingredientId} not found")
                .WithMetadata("ErrorCode", "INGREDIENT_NOT_FOUND")
                .WithMetadata("IngredientId", ingredientId.ToString())
                .WithMetadata("Type", "NotFound");


        public static Error KitchenAccessDenied(Guid kitchenId, Guid userId) =>
            new Error($"User does not have access to kitchen {kitchenId}")
                .WithMetadata("ErrorCode", "INGREDIENT_KITCHEN_ACCESS_DENIED")
                .WithMetadata("KitchenId", kitchenId.ToString())
                .WithMetadata("UserId", userId.ToString())
                .WithMetadata("Type", "Forbidden");
    }

    public static Error DatabaseError(string operation, Exception? ex = null) =>
        new Error($"Database error during {operation}")
            .WithMetadata("ErrorCode", "INGREDIENT_DATABASE_ERROR")
            .WithMetadata("Operation", operation)
            .WithMetadata("Type", "Internal")
            .CausedBy(ex);

    public static Error UnexpectedError(string operation, Exception? ex = null) =>
        new Error($"Unexpected error during {operation}")
            .WithMetadata("ErrorCode", "INGREDIENT_UNEXPECTED_ERROR")
            .WithMetadata("Operation", operation)
            .WithMetadata("Type", "Internal")
            .CausedBy(ex);
}
