using KitchenStock.Domain.ValueObjects;

namespace KitchenStock.Application.Modules.Supplier.Dtos;

public record SupplierResponseDto(
    Guid Id,
    string Name,
    Contact Contact,
    Address Address,
    int StockEntriesCount,
    decimal TotalPurchaseValue,
    DateTime? LastPurchaseDate,
    DateTime CreatedAt
);