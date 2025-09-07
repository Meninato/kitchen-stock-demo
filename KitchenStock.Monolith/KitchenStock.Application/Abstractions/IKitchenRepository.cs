using KitchenStock.Domain.Entities;

namespace KitchenStock.Application.Abstractions;

public interface IKitchenRepository : IGenericRepository<KitchenEntity>
{
    Task<KitchenEntity?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<KitchenEntity>> GetByUserIdAsync(Guid userId);
    Task<int> CountByUserIdAsync(Guid userId);
    Task<bool> UserOwnsKitchenAsync(Guid kitchenId, Guid userId);
}
