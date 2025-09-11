using KitchenStock.Domain.Enums;

namespace KitchenStock.Application.Modules.Stock.Dtos;

public record StockEntryResponseDto(
    Guid Id,
    string IngredientName,
    StockMovementType MovementType,
    decimal Quantity,
    decimal? UnitPrice,
    decimal? TotalValue,
    string SupplierName,
    string Reason,
    string Reference,
    DateTime MovementDate
);
