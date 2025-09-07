using KitchenStock.Domain.Common;
namespace KitchenStock.Domain.Entities;

public class KitchenEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int OwnerId { get; set; }
    public UserEntity Owner { get; set; } = null!;

    public ICollection<IngredientEntity> Ingredients { get; set; } = new List<IngredientEntity>();
    public ICollection<RecipeEntity> Recipes { get; set; } = new List<RecipeEntity>();
    public ICollection<SupplierEntity> Suppliers { get; set; } = new List<SupplierEntity>();
}