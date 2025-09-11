namespace KitchenStock.Application.Modules.Stock.Dtos;

public record StockIngredientResponseDto(
    Guid Id,
    string Name,
    string Description,
    string UnitOfMeasure,
    string UnitSymbol,
    decimal CurrentStock,
    decimal MinimumStock,
    bool IsLowStock,
    decimal LastUnitPrice,
    decimal AverageUnitPrice,
    DateTime CreatedAt
);
