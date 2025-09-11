namespace KitchenStock.Application.Modules.Ingredients.Dtos;

public record IngredientResponseDto(
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