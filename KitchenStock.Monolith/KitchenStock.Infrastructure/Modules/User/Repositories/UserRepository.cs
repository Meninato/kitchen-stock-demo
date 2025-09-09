using KitchenStock.Application.Modules.User.Abstractions;
using KitchenStock.Infrastructure.Persistence;
using KitchenStock.Domain.Entities;
using KitchenStock.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KitchenStock.Infrastructure.Modules.User.Repositories;

public class UserRepository : EfCoreGenericRepository<UserEntity>, IUserRepository
{
    public UserRepository(KitchenStockDbContext context) : base(context) { }

    public override async Task<UserEntity?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(u => u.Kitchens)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public override async Task<IEnumerable<UserEntity>> GetAllAsync()
    {
        return await _dbSet
            .Include(u => u.Kitchens)
            .ToListAsync();
    }

    public async Task<UserEntity?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(u => u.Kitchens)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _dbSet
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<int> GetKitchenCountAsync(Guid userId)
    {
        return await _context.Kitchens
            .CountAsync(k => k.OwnerId == userId);
    }
}