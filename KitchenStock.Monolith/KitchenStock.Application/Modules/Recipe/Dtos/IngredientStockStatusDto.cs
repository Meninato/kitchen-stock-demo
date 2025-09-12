namespace KitchenStock.Application.Modules.Recipe.Dtos;

public record IngredientStockStatusDto(
    Guid IngredientId,
    string IngredientName,
    decimal RequiredQuantity,
    decimal AvailableStock,
    decimal MaxPortionsFromThisIngredient,
    bool IsSufficient
);