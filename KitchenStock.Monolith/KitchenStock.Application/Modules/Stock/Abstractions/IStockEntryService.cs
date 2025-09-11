using FluentResults;
using KitchenStock.Application.Modules.Stock.Dtos;
using KitchenStock.Application.Modules.Stock.Results;
using KitchenStock.Domain.Enums;

namespace KitchenStock.Application.Modules.Stock.Abstractions;

public interface IStockEntryService
{
    Task<StockEntryResult> AddStockEntryAsync(Guid userId, CreateStockEntryDto request);
    Task<StockEntryResult> GetStockEntryByIdAsync(Guid id, Guid userId);
    Task<StockEntryListResult> GetIngredientStockHistoryAsync(Guid ingredientId, Guid userId);
    Task<StockEntryListResult> GetKitchenStockHistoryAsync(Guid kitchenId, Guid userId, DateTime? fromDate = null);
    Task<StockSummaryResult> GetStockSummaryAsync(Guid kitchenId, Guid userId);
    Task<Result<bool>> ProcessStockMovementAsync(Guid ingredientId, decimal quantity, StockMovementType type, string reason, Guid? supplierId = null);
}
