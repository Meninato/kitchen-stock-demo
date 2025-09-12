namespace KitchenStock.Application.Modules.Recipe.Dtos;

public record RecipeProductionResponseDto(
    Guid RecipeId,
    string RecipeName,
    decimal MaxProductionQuantity,
    List<IngredientStockStatusDto> IngredientStatus,
    bool CanProduce,
    string LimitingFactor
);
