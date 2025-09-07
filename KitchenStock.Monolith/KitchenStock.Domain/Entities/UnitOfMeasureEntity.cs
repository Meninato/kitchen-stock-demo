using KitchenStock.Domain.Common;

namespace KitchenStock.Domain.Entities;

public class UnitOfMeasureEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty; // quilograma, litro, unidade
    public string Symbol { get; set; } = string.Empty; // kg, L, un

    // Relacionamentos
    public ICollection<IngredientEntity> Ingredients { get; set; } = new List<IngredientEntity>();
}
