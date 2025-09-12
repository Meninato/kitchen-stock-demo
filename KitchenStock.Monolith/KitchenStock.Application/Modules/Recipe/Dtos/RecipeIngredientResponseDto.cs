namespace KitchenStock.Application.Modules.Recipe.Dtos;

public record RecipeIngredientResponseDto(
    Guid IngredientId,
    string IngredientName,
    string UnitSymbol,
    decimal Quantity,
    string Notes,
    decimal UnitPrice,
    decimal TotalCost,
    bool HasSufficientStock,
    decimal AvailableStock
);