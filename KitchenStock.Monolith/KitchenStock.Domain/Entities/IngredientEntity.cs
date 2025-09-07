using KitchenStock.Domain.Common;
using KitchenStock.Domain.Enums;

namespace KitchenStock.Domain.Entities;

public class IngredientEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Guid UnitOfMeasureId { get; set; }
    public UnitOfMeasureEntity UnitOfMeasure { get; set; } = null!;

    public decimal CurrentStock { get; set; } = 0;
    public decimal MinimumStock { get; set; } = 0;

    public Guid KitchenId { get; set; }
    public KitchenEntity Kitchen { get; set; } = null!;

    public ICollection<RecipeIngredientEntity> RecipeIngredients { get; set; } = new List<RecipeIngredientEntity>();
    public ICollection<StockEntryEntity> StockEntries { get; set; } = new List<StockEntryEntity>();

    public decimal AverageUnitPrice
    {
        get
        {
            var purchases = StockEntries
                .Where(se => se.MovementType == StockMovementType.Purchase && se.UnitPrice.HasValue)
                .ToList();

            return purchases.Any()
                ? purchases.Average(p => p.UnitPrice!.Value)
                : 0;
        }
    }

    public decimal LastUnitPrice
    {
        get
        {
            var lastPurchase = StockEntries
                .Where(se => se.MovementType == StockMovementType.Purchase && se.UnitPrice.HasValue)
                .OrderByDescending(se => se.MovementDate)
                .FirstOrDefault();

            return lastPurchase?.UnitPrice ?? 0;
        }
    }

    public bool IsLowStock => CurrentStock <= MinimumStock;
}