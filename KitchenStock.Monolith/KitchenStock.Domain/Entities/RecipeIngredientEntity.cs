using KitchenStock.Domain.Common;

namespace KitchenStock.Domain.Entities;

public class RecipeIngredientEntity : BaseEntity
{
    public Guid RecipeId { get; set; }
    public RecipeEntity Recipe { get; set; } = null!;

    public Guid IngredientId { get; set; }
    public IngredientEntity Ingredient { get; set; } = null!;

    public decimal Quantity { get; set; } = 0;

    public string Notes { get; set; } = string.Empty; // chop or cubes etc.. 

    public decimal TotalCost => Quantity * (Ingredient?.LastUnitPrice ?? 0);
}
