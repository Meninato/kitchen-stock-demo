using KitchenStock.Application.Abstractions;
using KitchenStock.Domain.Entities;

namespace KitchenStock.Application.Modules.Kitchen.Abstractions;

public interface IKitchenRepository : IGenericRepository<KitchenEntity>
{
    Task<KitchenEntity?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<KitchenEntity>> GetByUserIdAsync(Guid userId);
    Task<int> CountByUserIdAsync(Guid userId);
    Task<bool> UserOwnsKitchenAsync(Guid kitchenId, Guid userId);
    Task<bool> ExistsInUserAsync(string name, Guid userId, Guid? excludeId = null);
}
