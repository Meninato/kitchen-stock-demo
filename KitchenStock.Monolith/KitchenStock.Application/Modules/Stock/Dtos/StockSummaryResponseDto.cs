namespace KitchenStock.Application.Modules.Stock.Dtos;

public record StockSummaryResponseDto(
    decimal TotalStockValue,
    int TotalIngredients,
    int LowStockIngredients,
    int MovementsThisMonth,
    List<StockIngredientResponseDto> LowStockItems
);
