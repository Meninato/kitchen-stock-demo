namespace KitchenStock.Application.Modules.Recipe.Dtos;

public record IngredientCostBreakdownDto(
    string IngredientName,
    decimal Quantity,
    string Unit,
    decimal UnitPrice,
    decimal TotalCost,
    decimal PercentageOfTotal
);