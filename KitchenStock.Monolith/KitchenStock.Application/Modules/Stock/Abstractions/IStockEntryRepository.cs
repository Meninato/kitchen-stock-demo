using KitchenStock.Application.Abstractions;
using KitchenStock.Domain.Entities;
using KitchenStock.Domain.Enums;

namespace KitchenStock.Application.Modules.Stock.Abstractions;

public interface IStockEntryRepository : IGenericRepository<StockEntryEntity>
{
    Task<IEnumerable<StockEntryEntity>> GetByIngredientIdAsync(Guid ingredientId);
    Task<IEnumerable<StockEntryEntity>> GetByKitchenIdAsync(Guid kitchenId, DateTime? fromDate = null);
    Task<decimal> GetCurrentStockAsync(Guid ingredientId);
    Task<decimal?> GetLastUnitPriceAsync(Guid ingredientId);
    Task<decimal> GetAverageUnitPriceAsync(Guid ingredientId);
    Task<decimal> GetWeightedAverageUnitPriceAsync(Guid ingredientId);
    Task<IEnumerable<StockEntryEntity>> GetByMovementTypeAsync(Guid kitchenId, StockMovementType movementType, DateTime? fromDate = null);
    Task<decimal> GetTotalPurchaseValueAsync(Guid kitchenId, DateTime? fromDate = null);
    Task<Dictionary<string, decimal>> GetMonthlyPurchaseSummaryAsync(Guid kitchenId, int year);
}
