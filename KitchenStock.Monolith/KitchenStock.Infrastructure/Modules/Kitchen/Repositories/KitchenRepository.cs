using KitchenStock.Application.Modules.Kitchen.Abstractions;
using KitchenStock.Domain.Entities;
using KitchenStock.Infrastructure.Persistence;
using KitchenStock.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KitchenStock.Infrastructure.Modules.Kitchen.Repositories;

public class KitchenRepository : EfCoreGenericRepository<KitchenEntity>, IKitchenRepository
{
    public KitchenRepository(KitchenStockDbContext context) : base(context) { }

    public override async Task<KitchenEntity?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(k => k.Owner)
            .FirstOrDefaultAsync(k => k.Id == id);
    }

    public override async Task<IEnumerable<KitchenEntity>> GetAllAsync()
    {
        return await _dbSet
            .Include(k => k.Owner)
            .ToListAsync();
    }

    public async Task<KitchenEntity?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(k => k.Owner)
            .Include(k => k.Ingredients)
                .ThenInclude(i => i.UnitOfMeasure)
            .Include(k => k.Ingredients)
                .ThenInclude(i => i.StockEntries)
            .Include(k => k.Recipes)
                .ThenInclude(r => r.RecipeIngredients)
            .Include(k => k.Suppliers)
            .FirstOrDefaultAsync(k => k.Id == id);
        //return await _dbSet
        //    .Where(k => k.Id == id)
        //    .Select(k => new KitchenEntity
        //    {
        //        Id = k.Id,
        //        Name = k.Name,
        //        Owner = k.Owner,
        //        Suppliers = k.Suppliers.ToList(),
        //        Recipes = k.Recipes.Select(r => new Recipe
        //        {
        //            Id = r.Id,
        //            Name = r.Name,
        //            RecipeIngredients = r.RecipeIngredients.ToList()
        //        }).ToList(),
        //        Ingredients = k.Ingredients.Select(i => new Ingredient
        //        {
        //            Id = i.Id,
        //            Name = i.Name,
        //            UnitOfMeasure = i.UnitOfMeasure,
        //            StockEntries = i.StockEntries
        //                .OrderByDescending(se => se.MovementDate)
        //                .Take(5)
        //                .ToList()
        //        }).ToList()
        //    })
        //    .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<KitchenEntity>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(k => k.Ingredients)
            .Include(k => k.Recipes)
            .Where(k => k.OwnerId == userId)
            .OrderBy(k => k.Name)
            .ToListAsync();
    }

    public async Task<int> CountByUserIdAsync(Guid userId)
    {
        return await _context.Kitchens.CountAsync(k => k.OwnerId == userId);
    }

    public async Task<bool> UserOwnsKitchenAsync(Guid kitchenId, Guid userId)
    {
        return await _context.Kitchens.AnyAsync(k => k.Id == kitchenId && k.OwnerId == userId);
    }

    public async Task<bool> ExistsInUserAsync(string name, Guid userId, Guid? excludeId = null)
    {
        var query = _context.Kitchens
            .Where(k => k.OwnerId == userId &&
                       k.Name.ToLower() == name.ToLower());

        if (excludeId.HasValue)
            query = query.Where(k => k.Id != excludeId.Value);

        return await query.AnyAsync();
    }
}
