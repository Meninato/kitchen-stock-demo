namespace KitchenStock.Application.Modules.Ingredients.Dtos;

public record IngredientFilterDto(string? SearchTerm = null, bool LowStockOnly = false);
