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
        return await _context.Kitchens
            .Include(k => k.Owner)
            .FirstOrDefaultAsync(k => k.Id == id);
    }

    public async Task<KitchenEntity?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Kitchens
            .Include(k => k.Owner)
            .Include(k => k.Ingredients)
            .Include(k => k.Recipes)
            .Include(k => k.Suppliers)
            .FirstOrDefaultAsync(k => k.Id == id);
    }

    public async Task<IEnumerable<KitchenEntity>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Kitchens
            .Where(k => k.OwnerId == userId)
            .Include(k => k.Ingredients)
            .Include(k => k.Recipes)
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
}