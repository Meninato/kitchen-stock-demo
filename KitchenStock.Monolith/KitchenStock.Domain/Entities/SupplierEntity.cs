using KitchenStock.Domain.Common;
using KitchenStock.Domain.ValueObjects;

namespace KitchenStock.Domain.Entities;

public class SupplierEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Contact SupplierContact { get; set; } = null!;
    public Address SupplierAddress { get; set; } = null!;

    // Pertence a uma cozinha
    public Guid KitchenId { get; set; }
    public KitchenEntity Kitchen { get; set; } = null!;

    // Relacionamentos
    public ICollection<StockEntryEntity> StockEntries { get; set; } = new List<StockEntryEntity>();
}
