namespace KitchenStock.Application.Modules.Ingredients.Dtos;

public record CreateIngredientDto(
    Guid KitchenId,
    string Name,
    string Description,
    string Category,
    Guid UnitOfMeasureId,
    decimal MinimumStock
);
