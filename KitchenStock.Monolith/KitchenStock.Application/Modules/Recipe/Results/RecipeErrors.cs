using FluentResults;

namespace KitchenStock.Application.Modules.Recipe.Results;

public static class RecipeErrors
{
    public static class Validation
    {
        public static Error NameRequired =>
            new Error("Recipe name is required")
                .WithMetadata("ErrorCode", "RECIPE_NAME_REQUIRED")
                .WithMetadata("Field", "Name")
                .WithMetadata("Type", "Validation");

        public static Error NameTooLong(int maxLength) =>
            new Error($"Recipe name must not exceed {maxLength} characters")
                .WithMetadata("ErrorCode", "RECIPE_NAME_TOO_LONG")
                .WithMetadata("Field", "Name")
                .WithMetadata("Type", "Validation");

        public static Error DescriptionTooLong(int maxLength) =>
            new Error($"Recipe description must not exceed {maxLength} characters")
                .WithMetadata("ErrorCode", "RECIPE_DESCRIPTION_TOO_LONG")
                .WithMetadata("Field", "Description")
                .WithMetadata("Type", "Validation");

        public static Error InvalidYield =>
            new Error("Recipe yield must be greater than zero")
                .WithMetadata("ErrorCode", "RECIPE_INVALID_YIELD")
                .WithMetadata("Field", "Yield")
                .WithMetadata("Type", "Validation");

        public static Error InvalidPrepTime =>
            new Error("Preparation time must be greater than or equal to zero")
                .WithMetadata("ErrorCode", "RECIPE_INVALID_PREP_TIME")
                .WithMetadata("Field", "PrepTimeMinutes")
                .WithMetadata("Type", "Validation");

        public static Error InvalidSellingPrice =>
            new Error("Selling price must be greater than zero")
                .WithMetadata("ErrorCode", "RECIPE_INVALID_SELLING_PRICE")
                .WithMetadata("Field", "SellingPrice")
                .WithMetadata("Type", "Validation");

        public static Error InvalidIngredientQuantity(decimal quantity) =>
            new Error($"Ingredient quantity must be greater than zero: {quantity}")
                .WithMetadata("ErrorCode", "RECIPE_INVALID_INGREDIENT_QUANTITY")
                .WithMetadata("Field", "Quantity")
                .WithMetadata("Quantity", quantity.ToString())
                .WithMetadata("Type", "Validation");

        public static Error InstructionsTooLong(int maxLength) =>
            new Error($"Instructions must not exceed {maxLength} characters")
                .WithMetadata("ErrorCode", "RECIPE_INSTRUCTIONS_TOO_LONG")
                .WithMetadata("Field", "Instructions")
                .WithMetadata("Type", "Validation");
    }

    public static class BusinessRules
    {
        public static Error AlreadyExistsInKitchen(string name, Guid kitchenId) =>
            new Error($"Recipe '{name}' already exists in this kitchen")
                .WithMetadata("ErrorCode", "RECIPE_ALREADY_EXISTS")
                .WithMetadata("Field", "Name")
                .WithMetadata("RecipeName", name)
                .WithMetadata("KitchenId", kitchenId.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error NoIngredientsProvided =>
            new Error("Recipe must have at least one ingredient")
                .WithMetadata("ErrorCode", "RECIPE_NO_INGREDIENTS")
                .WithMetadata("Type", "BusinessRule");

        public static Error DuplicateIngredient(Guid ingredientId) =>
            new Error($"Ingredient {ingredientId} is already in this recipe")
                .WithMetadata("ErrorCode", "RECIPE_DUPLICATE_INGREDIENT")
                .WithMetadata("IngredientId", ingredientId.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error IngredientNotInKitchen(Guid ingredientId, Guid kitchenId) =>
            new Error($"Ingredient {ingredientId} does not belong to kitchen {kitchenId}")
                .WithMetadata("ErrorCode", "RECIPE_INGREDIENT_NOT_IN_KITCHEN")
                .WithMetadata("IngredientId", ingredientId.ToString())
                .WithMetadata("KitchenId", kitchenId.ToString())
                .WithMetadata("Type", "BusinessRule");

        public static Error CannotCalculateCostInsufficientPriceData(string ingredientName) =>
            new Error($"Cannot calculate cost for ingredient '{ingredientName}' - no price data available")
                .WithMetadata("ErrorCode", "RECIPE_INSUFFICIENT_PRICE_DATA")
                .WithMetadata("IngredientName", ingredientName)
                .WithMetadata("Type", "BusinessRule");

        public static Error InsufficientStockForProduction(string recipeName, List<string> missingIngredients) =>
            new Error($"Insufficient stock to produce '{recipeName}'. Missing: {string.Join(", ", missingIngredients)}")
                .WithMetadata("ErrorCode", "RECIPE_INSUFFICIENT_STOCK")
                .WithMetadata("RecipeName", recipeName)
                .WithMetadata("MissingIngredients", string.Join(",", missingIngredients))
                .WithMetadata("Type", "BusinessRule");
    }

    public static class Authorization
    {
        public static Error NotFound(Guid recipeId) =>
            new Error($"Recipe with ID {recipeId} not found")
                .WithMetadata("ErrorCode", "RECIPE_NOT_FOUND")
                .WithMetadata("RecipeId", recipeId.ToString())
                .WithMetadata("Type", "NotFound");

        public static Error KitchenAccessDenied(Guid kitchenId, Guid userId) =>
            new Error($"User does not have access to kitchen {kitchenId}")
                .WithMetadata("ErrorCode", "RECIPE_KITCHEN_ACCESS_DENIED")
                .WithMetadata("KitchenId", kitchenId.ToString())
                .WithMetadata("UserId", userId.ToString())
                .WithMetadata("Type", "Forbidden");
    }

    public static Error DatabaseError(string operation, Exception? ex = null) =>
        new Error($"Database error during {operation}")
            .WithMetadata("ErrorCode", "RECIPE_DATABASE_ERROR")
            .WithMetadata("Operation", operation)
            .WithMetadata("Type", "Internal")
            .CausedBy(ex);

    public static Error UnexpectedError(string operation, Exception? ex = null) =>
        new Error($"Unexpected error during {operation}")
            .WithMetadata("ErrorCode", "RECIPE_UNEXPECTED_ERROR")
            .WithMetadata("Operation", operation)
            .WithMetadata("Type", "Internal")
            .CausedBy(ex);
}