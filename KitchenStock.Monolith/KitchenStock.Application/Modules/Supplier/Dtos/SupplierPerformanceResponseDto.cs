namespace KitchenStock.Application.Modules.Supplier.Dtos;

public record SupplierPerformanceResponseDto(
    Guid SupplierId,
    string SupplierName,
    int TotalPurchases,
    decimal TotalValue,
    decimal AverageOrderValue,
    DateTime? FirstPurchase,
    DateTime? LastPurchase,
    List<MonthlyPurchaseSummaryDto> MonthlyBreakdown,
    List<IngredientPurchaseSummaryDto> TopIngredients
);