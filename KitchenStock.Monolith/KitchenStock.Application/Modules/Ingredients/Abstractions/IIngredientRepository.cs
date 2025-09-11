using KitchenStock.Application.Abstractions;
using KitchenStock.Domain.Entities;

namespace KitchenStock.Application.Modules.Ingredients.Abstractions;

public interface IIngredientRepository : IGenericRepository<IngredientEntity>
{
    Task<IngredientEntity?> GetByIdWithStockHistoryAsync(Guid id);
    Task<IEnumerable<IngredientEntity>> GetByKitchenIdAsync(Guid kitchenId);
    Task<IEnumerable<IngredientEntity>> GetLowStockByKitchenIdAsync(Guid kitchenId);
    Task<bool> ExistsInKitchenAsync(string name, Guid kitchenId, Guid? excludeId = null);
    Task<IEnumerable<IngredientEntity>> GetByIdsAsync(Guid[] ingredientIds);
    Task<IEnumerable<IngredientEntity>> SearchByNameAsync(string searchTerm, Guid kitchenId);
    Task<decimal> GetTotalStockValueAsync(Guid kitchenId);
}
