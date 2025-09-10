namespace KitchenStock.Application.Modules.Kitchen.Dtos;

public record KitchenStatsResponseDto(
    int TotalIngredients,
    int LowStockIngredients,
    int TotalRecipes,
    decimal TotalStockValue,
    int RecentMovements
);
