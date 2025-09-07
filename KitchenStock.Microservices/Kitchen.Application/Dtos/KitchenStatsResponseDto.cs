namespace Kitchen.Application.Dtos;

public record KitchenStatsResponseDto(
    Guid KitchenId,
    string KitchenName,
    int TotalIngredients,
    int LowStockIngredients,
    int TotalRecipes,
    int TotalSuppliers,
    decimal? EstimatedInventoryValue,
    DateTime LastUpdated
);
