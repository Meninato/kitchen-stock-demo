namespace KitchenStock.Application.Modules.Supplier.Dtos;

public record IngredientPurchaseSummaryDto(
    string IngredientName,
    decimal TotalQuantity,
    string Unit,
    decimal TotalValue,
    decimal AveragePrice
);