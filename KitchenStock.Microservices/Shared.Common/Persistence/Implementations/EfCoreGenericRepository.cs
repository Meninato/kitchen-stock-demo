using Microsoft.EntityFrameworkCore;
using Shared.Common.Persistence.Abstractions;

namespace Shared.Common.Persistence.Implementations;

public class EfCoreGenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly BaseDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public EfCoreGenericRepository(BaseDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return false;

        entity.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public virtual async Task<bool> DeletePermanentlyAsync(Guid id)
    {
        var entity = await _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == id);
        if (entity == null) return false;

        _dbSet.Remove(entity);
        await _context.SaveChangesWithHardDeleteAsync();
        return true;
    }

    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(e => e.Id == id);
    }
}
