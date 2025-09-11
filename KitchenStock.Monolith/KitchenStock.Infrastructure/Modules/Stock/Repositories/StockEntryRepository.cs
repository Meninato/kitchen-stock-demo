using KitchenStock.Application.Modules.Stock.Abstractions;
using KitchenStock.Domain.Entities;
using KitchenStock.Domain.Enums;
using KitchenStock.Infrastructure.Persistence;
using KitchenStock.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KitchenStock.Infrastructure.Modules.Stock.Repositories;

public class StockEntryRepository : EfCoreGenericRepository<StockEntryEntity>, IStockEntryRepository
{
    public StockEntryRepository(KitchenStockDbContext context) : base(context) { }

    public override async Task<StockEntryEntity?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(se => se.Ingredient)
                .ThenInclude(i => i.Kitchen)
            .Include(se => se.Ingredient)
                .ThenInclude(i => i.UnitOfMeasure)
            .Include(se => se.Supplier)
            .FirstOrDefaultAsync(se => se.Id == id);
    }

    public override async Task<IEnumerable<StockEntryEntity>> GetAllAsync()
    {
        return await _dbSet
            .Include(se => se.Ingredient)
                .ThenInclude(i => i.Kitchen)
            .Include(se => se.Ingredient)
                .ThenInclude(i => i.UnitOfMeasure)
            .Include(se => se.Supplier)
            .ToListAsync();
    }

    public async Task<IEnumerable<StockEntryEntity>> GetByIngredientIdAsync(Guid ingredientId)
    {
        return await _dbSet
            .Include(se => se.Supplier)
            .Where(se => se.IngredientId == ingredientId)
            .OrderByDescending(se => se.MovementDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<StockEntryEntity>> GetByKitchenIdAsync(Guid kitchenId, DateTime? fromDate = null)
    {
        var query = _dbSet
            .Include(se => se.Ingredient)
                .ThenInclude(i => i.UnitOfMeasure)
            .Include(se => se.Supplier)
            .Where(se => se.Ingredient.KitchenId == kitchenId);

        if (fromDate.HasValue)
            query = query.Where(se => se.MovementDate >= fromDate.Value);

        return await query
            .OrderByDescending(se => se.MovementDate)
            .ThenByDescending(se => se.CreatedAt)
            .ToListAsync();
    }

    public async Task<decimal> GetCurrentStockAsync(Guid ingredientId)
    {
        var ingredient = await _context.Ingredients.FindAsync(ingredientId);
        return ingredient?.CurrentStock ?? 0;
    }

    public async Task<decimal?> GetLastUnitPriceAsync(Guid ingredientId)
    {
        var lastPurchase = await _dbSet
            .Where(se => se.IngredientId == ingredientId &&
                        se.MovementType == StockMovementType.Purchase &&
                        se.UnitPrice.HasValue)
            .OrderByDescending(se => se.MovementDate)
            .ThenByDescending(se => se.CreatedAt)
            .FirstOrDefaultAsync();

        return lastPurchase?.UnitPrice;
    }

    public async Task<decimal> GetAverageUnitPriceAsync(Guid ingredientId)
    {
        var purchases = await _dbSet
            .Where(se => se.IngredientId == ingredientId &&
                        se.MovementType == StockMovementType.Purchase &&
                        se.UnitPrice.HasValue)
            .Select(se => se.UnitPrice!.Value)
            .ToListAsync();

        return purchases.Any() ? purchases.Average() : 0;
    }

    public async Task<decimal> GetWeightedAverageUnitPriceAsync(Guid ingredientId)
    {
        var purchases = await _dbSet
            .Where(se => se.IngredientId == ingredientId &&
                        se.MovementType == StockMovementType.Purchase &&
                        se.UnitPrice.HasValue)
            .Select(se => new { se.Quantity, se.UnitPrice })
            .ToListAsync();

        if (!purchases.Any()) return 0;

        var totalQuantity = purchases.Sum(p => p.Quantity);
        var totalValue = purchases.Sum(p => p.Quantity * p.UnitPrice!.Value);

        return totalQuantity > 0 ? totalValue / totalQuantity : 0;
    }

    public async Task<IEnumerable<StockEntryEntity>> GetByMovementTypeAsync(Guid kitchenId, StockMovementType movementType, DateTime? fromDate = null)
    {
        var query = _dbSet
            .Include(se => se.Ingredient)
            .Include(se => se.Supplier)
            .Where(se => se.Ingredient.KitchenId == kitchenId && se.MovementType == movementType);

        if (fromDate.HasValue)
            query = query.Where(se => se.MovementDate >= fromDate.Value);

        return await query
            .OrderByDescending(se => se.MovementDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalPurchaseValueAsync(Guid kitchenId, DateTime? fromDate = null)
    {
        var query = _dbSet
            .Where(se => se.Ingredient.KitchenId == kitchenId &&
                        se.MovementType == StockMovementType.Purchase &&
                        se.UnitPrice.HasValue);

        if (fromDate.HasValue)
            query = query.Where(se => se.MovementDate >= fromDate.Value);

        var purchases = await query.ToListAsync();
        return purchases.Sum(se => se.Quantity * se.UnitPrice!.Value);
    }

    public async Task<Dictionary<string, decimal>> GetMonthlyPurchaseSummaryAsync(Guid kitchenId, int year)
    {
        var purchases = await _dbSet
            .Where(se => se.Ingredient.KitchenId == kitchenId &&
                        se.MovementType == StockMovementType.Purchase &&
                        se.UnitPrice.HasValue &&
                        se.MovementDate.Year == year)
            .ToListAsync();

        return purchases
            .GroupBy(se => se.MovementDate.ToString("yyyy-MM"))
            .ToDictionary(
                g => g.Key,
                g => g.Sum(se => se.Quantity * se.UnitPrice!.Value)
            );
    }
}
