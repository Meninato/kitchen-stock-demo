using KitchenStock.Application.Modules.Recipe.Abstractions;
using KitchenStock.Domain.Entities;
using KitchenStock.Infrastructure.Persistence;
using KitchenStock.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KitchenStock.Infrastructure.Modules.Recipe.Repositories;

public class RecipeRepository : EfCoreGenericRepository<RecipeEntity>, IRecipeRepository
{

    public RecipeRepository(KitchenStockDbContext context) : base(context) { }

    public override async Task<RecipeEntity?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(r => r.Kitchen)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public override async Task<IEnumerable<RecipeEntity>> GetAllAsync()
    {
        return await _dbSet
            .Include(r => r.Kitchen)
            .ToListAsync();
    }

    public async Task<RecipeEntity?> GetByIdWithIngredientsAsync(Guid id)
    {
        return await _dbSet
            .Include(r => r.Kitchen)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                    .ThenInclude(i => i.UnitOfMeasure)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                    .ThenInclude(i => i.StockEntries)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<RecipeEntity>> GetByKitchenIdAsync(Guid kitchenId)
    {
        return await _dbSet
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                    .ThenInclude(i => i.UnitOfMeasure)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                    .ThenInclude(i => i.StockEntries)
            .Where(r => r.KitchenId == kitchenId)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<bool> ExistsInKitchenAsync(string name, Guid kitchenId, Guid? excludeId = null)
    {
        var query = _dbSet
            .Where(r => r.KitchenId == kitchenId &&
                       r.Name.ToLower() == name.ToLower());

        if (excludeId.HasValue)
            query = query.Where(r => r.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<RecipeEntity>> SearchByNameAsync(string searchTerm, Guid kitchenId)
    {
        return await _dbSet
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Where(r => r.KitchenId == kitchenId &&
                       r.Name.ToLower().Contains(searchTerm.ToLower()))
            .OrderBy(r => r.Name)
            .Take(10)
            .ToListAsync();
    }

    public async Task<decimal> GetKitchenRecipesTotalValueAsync(Guid kitchenId)
    {
        var recipes = await _dbSet
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                    .ThenInclude(i => i.StockEntries)
            .Where(r => r.KitchenId == kitchenId)
            .ToListAsync();

        decimal totalValue = 0;
        foreach (var recipe in recipes)
        {
            totalValue += recipe.TotalCost;
        }

        return totalValue;
    }

    public async Task<IEnumerable<RecipeEntity>> GetProfitableRecipesAsync(Guid kitchenId, decimal minMarginPercent)
    {
        var recipes = await GetByKitchenIdAsync(kitchenId);

        return recipes.Where(r => r.ProfitMargin.HasValue && r.ProfitMargin.Value >= minMarginPercent);
    }
}