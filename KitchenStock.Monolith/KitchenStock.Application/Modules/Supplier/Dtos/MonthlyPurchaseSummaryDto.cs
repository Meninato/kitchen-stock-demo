namespace KitchenStock.Application.Modules.Supplier.Dtos;

public record MonthlyPurchaseSummaryDto(
    string Month,
    int PurchaseCount,
    decimal TotalValue
);