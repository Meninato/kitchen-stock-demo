using KitchenStock.Application.Abstractions;
using KitchenStock.Domain.Entities;
using KitchenStock.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KitchenStock.Infrastructure.Repositories;

public class UserRepository : EfCoreGenericRepository<UserEntity>, IUserRepository
{
    public UserRepository(KitchenStockDbContext context) : base(context) { }

    public async Task<UserEntity?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }
}