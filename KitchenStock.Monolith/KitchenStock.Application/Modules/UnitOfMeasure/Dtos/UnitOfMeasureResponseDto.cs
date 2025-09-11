namespace KitchenStock.Application.Modules.UnitOfMeasure.Dtos;

public record UnitOfMeasureResponseDto(
    Guid Id,
    string Name,
    string Symbol,
    int IngredientCount,
    bool IsSystemUnit
);