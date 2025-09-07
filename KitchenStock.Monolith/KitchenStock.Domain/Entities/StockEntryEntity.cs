using KitchenStock.Domain.Common;
using KitchenStock.Domain.Enums;

namespace KitchenStock.Domain.Entities;

public class StockEntryEntity : BaseEntity
{
    public Guid IngredientId { get; set; }
    public IngredientEntity Ingredient { get; set; } = null!;

    public StockMovementType MovementType { get; set; }

    // Quantidade (positiva para entrada, negativa para saída)
    public decimal Quantity { get; set; } = 0;

    // Preço unitário (só para compras)
    public decimal? UnitPrice { get; set; }

    // Fornecedor (opcional)
    public Guid? SupplierId { get; set; }
    public SupplierEntity? Supplier { get; set; }

    public string Reason { get; set; } = string.Empty;

    public DateTime MovementDate { get; set; } = DateTime.UtcNow;

    public string Reference { get; set; } = string.Empty; // NF, pedido, etc.

    // Propriedade calculada
    public decimal? TotalValue => UnitPrice.HasValue ? Quantity * UnitPrice.Value : null;
}