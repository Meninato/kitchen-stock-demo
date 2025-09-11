using KitchenStock.Domain.Enums;

namespace KitchenStock.Application.Modules.Stock.Dtos;

public record CreateStockEntryDto(
    Guid IngredientId,
    StockMovementType MovementType,
    decimal Quantity,
    decimal? UnitPrice,
    Guid? SupplierId,
    string Reason,
    string Reference
);
