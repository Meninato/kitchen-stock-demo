namespace KitchenStock.Application.Modules.Recipe.Dtos;

public record RecipeIngredientDto(
    Guid IngredientId,
    decimal Quantity,
    string Notes
);