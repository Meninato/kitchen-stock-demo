using KitchenStock.Domain.Common;

namespace KitchenStock.Domain.Entities;

public class RecipeEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Yield { get; set; } 
    public int PrepTimeMinutes { get; set; }
    public string Instructions { get; set; } = string.Empty;
    public decimal? SellingPrice { get; set; }

    public Guid KitchenId { get; set; }
    public KitchenEntity Kitchen { get; set; } = null!;

    public ICollection<RecipeIngredientEntity> RecipeIngredients { get; set; } = new List<RecipeIngredientEntity>();

    public decimal TotalCost => RecipeIngredients.Sum(ri => ri.TotalCost);
    public decimal CostPerPortion => Yield > 0 ? TotalCost / Yield : 0;

    public decimal? ProfitMargin => SellingPrice.HasValue && TotalCost > 0
        ? ((SellingPrice.Value - CostPerPortion) / SellingPrice.Value) * 100
        : null;
}
