using KitchenStock.Application.Abstractions;
using KitchenStock.Domain.Entities;
using KitchenStock.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KitchenStock.Infrastructure.Repositories;

public class KitchenRepository : EfCoreGenericRepository<KitchenEntity>, IKitchenRepository
{
    public KitchenRepository(KitchenStockDbContext context) : base(context) { }

    public override async Task<KitchenEntity?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(k => k.Owner)
            .FirstOrDefaultAsync(k => k.Id == id);
    }

    public async Task<KitchenEntity?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(k => k.Owner)
            .Include(k => k.Ingredients)
            .Include(k => k.Recipes)
            .Include(k => k.Suppliers)
            .FirstOrDefaultAsync(k => k.Id == id);
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
        return await _dbSet.CountAsync(k => k.OwnerId == userId);
    }

    public async Task<bool> UserOwnsKitchenAsync(Guid kitchenId, Guid userId)
    {
        return await _dbSet.AnyAsync(k => k.Id == kitchenId && k.OwnerId == userId);
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