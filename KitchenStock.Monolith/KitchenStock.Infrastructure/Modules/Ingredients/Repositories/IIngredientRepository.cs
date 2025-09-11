using KitchenStock.Application.Modules.Ingredients.Abstractions;
using KitchenStock.Domain.Entities;
using KitchenStock.Domain.Enums;
using KitchenStock.Infrastructure.Persistence;
using KitchenStock.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KitchenStock.Infrastructure.Modules.Ingredients.Repositories;

public class IngredientRepository : EfCoreGenericRepository<IngredientEntity>, IIngredientRepository
{
    public IngredientRepository(KitchenStockDbContext context) : base(context) { }

    public override async Task<IngredientEntity?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(i => i.UnitOfMeasure)
            .Include(i => i.Kitchen)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public override async Task<IEnumerable<IngredientEntity>> GetAllAsync()
    {
        return await _dbSet
            .Include(i => i.UnitOfMeasure)
            .Include(i => i.Kitchen)
            .ToListAsync();
    }

    public async Task<IngredientEntity?> GetByIdWithStockHistoryAsync(Guid id)
    {
        return await _dbSet
            .Include(i => i.UnitOfMeasure)
            .Include(i => i.Kitchen)
            .Include(i => i.StockEntries)
                .ThenInclude(se => se.Supplier)
            .Include(i => i.RecipeIngredients)
                .ThenInclude(ri => ri.Recipe)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<IngredientEntity>> GetByKitchenIdAsync(Guid kitchenId)
    {
        return await _dbSet
            .Include(i => i.UnitOfMeasure)
            .Include(i => i.StockEntries)
            .Where(i => i.KitchenId == kitchenId)
            .OrderBy(i => i.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<IngredientEntity>> GetLowStockByKitchenIdAsync(Guid kitchenId)
    {
        return await _dbSet
            .Include(i => i.UnitOfMeasure)
            .Where(i => i.KitchenId == kitchenId && i.CurrentStock <= i.MinimumStock)
            .OrderBy(i => i.CurrentStock / (i.MinimumStock == 0 ? 1 : i.MinimumStock))
            .ThenBy(i => i.Name)
            .ToListAsync();
    }

    public async Task<bool> ExistsInKitchenAsync(string name, Guid kitchenId, Guid? excludeId = null)
    {
        var query = _dbSet
            .Where(i => i.KitchenId == kitchenId &&
                       i.Name.ToLower() == name.ToLower());

        if (excludeId.HasValue)
            query = query.Where(i => i.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<IngredientEntity>> GetByIdsAsync(Guid[] ingredientIds)
    {
        return await _dbSet
            .Include(i => i.UnitOfMeasure)
            .Where(i => ingredientIds.Contains(i.Id))
            .ToListAsync();
    }

    public async Task<IEnumerable<IngredientEntity>> SearchByNameAsync(string searchTerm, Guid kitchenId)
    {
        return await _dbSet
            .Include(i => i.UnitOfMeasure)
            .Where(i => i.KitchenId == kitchenId &&
                       i.Name.ToLower().Contains(searchTerm.ToLower()))
            .OrderBy(i => i.Name)
            .Take(10)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalStockValueAsync(Guid kitchenId)
    {
        var ingredients = await _context.Ingredients
            .Include(i => i.StockEntries)
            .Where(i => i.KitchenId == kitchenId)
            .ToListAsync();

        decimal totalValue = 0;
        foreach (var ingredient in ingredients)
        {
            var lastPurchase = ingredient.StockEntries?
                .Where(se => se.MovementType == StockMovementType.Purchase && se.UnitPrice.HasValue)
                .OrderByDescending(se => se.MovementDate)
                .FirstOrDefault();

            if (lastPurchase != null && lastPurchase.UnitPrice.HasValue)
            {
                totalValue += ingredient.CurrentStock * lastPurchase.UnitPrice.Value;
            }
        }

        return totalValue;
    }
}
