using KitchenStock.Application.Modules.Supplier.Dtos;
using KitchenStock.Application.Modules.Supplier.Results;

namespace KitchenStock.Application.Modules.Supplier.Abstractions;

public interface ISupplierService
{
    Task<SupplierResult> CreateSupplierAsync(Guid userId, CreateSupplierDto request);
    Task<SupplierResult> GetSupplierByIdAsync(Guid id, Guid userId);
    Task<SupplierListResult> GetKitchenSuppliersAsync(Guid kitchenId, Guid userId);
    Task<SupplierResult> UpdateSupplierAsync(Guid id, Guid userId, UpdateSupplierDto request);
    Task<SupplierResult> DeleteSupplierAsync(Guid id, Guid userId);
    Task<SupplierPerformanceResult> GetSupplierPerformanceAsync(Guid supplierId, Guid userId, DateTime? fromDate = null);
    Task<SupplierListResult> SearchSuppliersAsync(string searchTerm, Guid kitchenId, Guid userId);
}
