namespace KitchenStock.Application.Modules.Ingredients.Dtos;

public record UpdateStockDto(
    decimal NewStock,
    string Reason
);
