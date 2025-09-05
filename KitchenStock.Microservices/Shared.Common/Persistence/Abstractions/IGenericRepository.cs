namespace Shared.Common.Persistence.Abstractions;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(Guid id); // Soft delete
    Task<bool> DeletePermanentlyAsync(Guid id); // Hard delete
    Task<bool> ExistsAsync(Guid id);
}
