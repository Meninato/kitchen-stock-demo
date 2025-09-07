using Kitchen.Domain.Entities;
using Shared.Common.Persistence.Abstractions;

namespace Kitchen.Application.Services.Abstractions;

public interface IKitchenRepository : IGenericRepository<KitchenEntity>
{
    Task<IEnumerable<KitchenEntity>> GetByOwnerIdAsync(Guid ownerId, bool includeInactive = false);
    Task<int> CountByOwnerIdAsync(Guid ownerId);
    Task<bool> IsOwnerAsync(Guid kitchenId, Guid userId);
    Task<KitchenEntity?> GetByIdAndOwnerAsync(Guid kitchenId, Guid ownerId);
}
