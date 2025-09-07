namespace KitchenStock.Application.Kitchen.Dtos;

public record KitchenResponseDto(
    Guid Id,
    string Name,
    string Description,
    int IngredientCount,
    int RecipeCount,
    int LowStockItems,
    DateTime CreatedAt
);
