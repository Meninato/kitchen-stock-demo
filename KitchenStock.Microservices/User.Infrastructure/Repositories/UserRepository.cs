using Microsoft.EntityFrameworkCore;
using Shared.Common.Persistence.Implementations;
using User.Application.Services.Abstractions;
using User.Domain.Entities;

namespace User.Infrastructure.Repositories;

public class UserRepository : EfCoreGenericRepository<UserEntity>, IUserRepository
{
    public UserRepository(UserDbContext context) : base(context) { }

    public async Task<UserEntity?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }
}
