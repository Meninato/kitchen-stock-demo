using KitchenStock.Application.Modules.UnitOfMeasure.Abstractions;
using KitchenStock.Domain.Entities;
using KitchenStock.Infrastructure.Persistence;
using KitchenStock.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KitchenStock.Infrastructure.Modules.UnitOfMeasure.Repositories;

public class UnitOfMeasureRepository : EfCoreGenericRepository<UnitOfMeasureEntity>, IUnitOfMeasureRepository
{
    public UnitOfMeasureRepository(KitchenStockDbContext context) : base(context) { }

    public override async Task<UnitOfMeasureEntity?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(u => u.Ingredients)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public override async Task<IEnumerable<UnitOfMeasureEntity>> GetAllAsync()
    {
        return await _dbSet
            .Include(u => u.Ingredients)
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    public async Task<bool> ExistsBySymbolAsync(string symbol, Guid? excludeId = null)
    {
        var query = _dbSet
            .Where(u => u.Symbol.ToLower() == symbol.ToLower());

        if (excludeId.HasValue)
            query = query.Where(u => u.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
    {
        var query = _dbSet
            .Where(u => u.Name.ToLower() == name.ToLower());

        if (excludeId.HasValue)
            query = query.Where(u => u.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<int> GetIngredientCountAsync(Guid unitId)
    {
        return await _context.Ingredients
            .CountAsync(i => i.UnitOfMeasureId == unitId);
    }
}