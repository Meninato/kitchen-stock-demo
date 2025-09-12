using KitchenStock.Application.Modules.Supplier.Abstractions;
using KitchenStock.Domain.Entities;
using KitchenStock.Domain.Enums;
using KitchenStock.Infrastructure.Persistence;
using KitchenStock.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace KitchenStock.Infrastructure.Modules.Supplier.Repositories;

public class SupplierRepository : EfCoreGenericRepository<SupplierEntity>, ISupplierRepository
{
    public SupplierRepository(KitchenStockDbContext context) : base(context) { }

    public override async Task<SupplierEntity?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(s => s.Kitchen)
            .Include(s => s.StockEntries)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public override async Task<IEnumerable<SupplierEntity>> GetAllAsync()
    {
        return await _dbSet
            .Include(s => s.Kitchen)
            .Include(s => s.StockEntries)
            .ToListAsync();
    }

    public async Task<SupplierEntity?> GetByIdWithStockEntriesAsync(Guid id)
    {
        return await _dbSet
            .Include(s => s.Kitchen)
            .Include(s => s.StockEntries)
                .ThenInclude(se => se.Ingredient)
                    .ThenInclude(i => i.UnitOfMeasure)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<SupplierEntity>> GetByKitchenIdAsync(Guid kitchenId)
    {
        return await _dbSet
            .Include(s => s.StockEntries)
            .Where(s => s.KitchenId == kitchenId)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<bool> ExistsInKitchenAsync(string name, Guid kitchenId, Guid? excludeId = null)
    {
        var query = _dbSet
            .Where(s => s.KitchenId == kitchenId &&
                       s.Name.ToLower() == name.ToLower());

        if (excludeId.HasValue)
            query = query.Where(s => s.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<bool> EmailExistsInKitchenAsync(string email, Guid kitchenId, Guid? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;

        var query = _dbSet
            .Where(s => s.KitchenId == kitchenId &&
                s.SupplierContact.Email != null &&
                s.SupplierContact.Email.ToLower() == email.ToLower());

        if (excludeId.HasValue)
            query = query.Where(s => s.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<bool> PhoneExistsInKitchenAsync(string phone, Guid kitchenId, Guid? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(phone)) return false;

        var normalizedPhone = NormalizePhone(phone);
        var query = _dbSet
            .Where(s => s.KitchenId == kitchenId);

        if (excludeId.HasValue)
            query = query.Where(s => s.Id != excludeId.Value);

        var suppliers = await query.ToListAsync();
        return suppliers.Any(s => NormalizePhone(s.SupplierContact.Phone) == normalizedPhone);
    }

    public async Task<IEnumerable<SupplierEntity>> SearchByNameAsync(string searchTerm, Guid kitchenId)
    {
        return await _dbSet
            .Include(s => s.StockEntries)
            .Where(s => s.KitchenId == kitchenId &&
                       s.Name.ToLower().Contains(searchTerm.ToLower()))
            .OrderBy(s => s.Name)
            .Take(10)
            .ToListAsync();
    }

    public async Task<IEnumerable<SupplierEntity>> GetTopSuppliersByVolumeAsync(Guid kitchenId, int topCount = 5)
    {
        var suppliers = await _dbSet
            .Include(s => s.StockEntries)
            .Where(s => s.KitchenId == kitchenId)
            .ToListAsync();

        return suppliers
            .OrderByDescending(s => s.StockEntries?
                .Where(se => se.MovementType == StockMovementType.Purchase && se.UnitPrice.HasValue)
                .Sum(se => se.Quantity * se.UnitPrice!.Value) ?? 0)
            .Take(topCount);
    }

    public async Task<Dictionary<Guid, decimal>> GetSupplierPurchaseVolumeAsync(Guid kitchenId, DateTime? fromDate = null)
    {
        var query = _context.StockEntries
            .Include(se => se.Supplier)
            .Where(se => se.Supplier != null &&
                        se.Supplier.KitchenId == kitchenId &&
                        se.MovementType == StockMovementType.Purchase &&
                        se.UnitPrice.HasValue);

        if (fromDate.HasValue)
            query = query.Where(se => se.MovementDate >= fromDate.Value);

        var stockEntries = await query.ToListAsync();

        return stockEntries
            .GroupBy(se => se.SupplierId!.Value)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(se => se.Quantity * se.UnitPrice!.Value)
            );
    }

    private string NormalizePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return "";
        return Regex.Replace(phone, @"[\s\-\(\)\.]", "");
    }
}