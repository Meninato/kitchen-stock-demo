namespace KitchenStock.Application.Modules.Ingredients.Dtos;

public record UpdateIngredientDto(
    string Name,
    string Description,
    string Category,
    Guid UnitOfMeasureId,
    decimal MinimumStock
);
