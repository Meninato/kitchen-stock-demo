using KitchenStock.Application.Abstractions;
using KitchenStock.Domain.Entities;

namespace KitchenStock.Application.Modules.Supplier.Abstractions;

public interface ISupplierRepository : IGenericRepository<SupplierEntity>
{
    Task<SupplierEntity?> GetByIdWithStockEntriesAsync(Guid id);
    Task<IEnumerable<SupplierEntity>> GetByKitchenIdAsync(Guid kitchenId);
    Task<bool> ExistsInKitchenAsync(string name, Guid kitchenId, Guid? excludeId = null);
    Task<bool> EmailExistsInKitchenAsync(string email, Guid kitchenId, Guid? excludeId = null);
    Task<bool> PhoneExistsInKitchenAsync(string phone, Guid kitchenId, Guid? excludeId = null);
    Task<IEnumerable<SupplierEntity>> SearchByNameAsync(string searchTerm, Guid kitchenId);
    Task<IEnumerable<SupplierEntity>> GetTopSuppliersByVolumeAsync(Guid kitchenId, int topCount = 5);
    Task<Dictionary<Guid, decimal>> GetSupplierPurchaseVolumeAsync(Guid kitchenId, DateTime? fromDate = null);
}
