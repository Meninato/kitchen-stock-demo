using KitchenStock.Application.Abstractions;
using KitchenStock.Domain.Entities;

namespace KitchenStock.Application.Modules.Recipe.Abstractions;

public interface IRecipeRepository : IGenericRepository<RecipeEntity>
{
    Task<RecipeEntity?> GetByIdWithIngredientsAsync(Guid id);
    Task<IEnumerable<RecipeEntity>> GetByKitchenIdAsync(Guid kitchenId);
    Task<bool> ExistsInKitchenAsync(string name, Guid kitchenId, Guid? excludeId = null);
    Task<IEnumerable<RecipeEntity>> SearchByNameAsync(string searchTerm, Guid kitchenId);
    Task<decimal> GetKitchenRecipesTotalValueAsync(Guid kitchenId);
    Task<IEnumerable<RecipeEntity>> GetProfitableRecipesAsync(Guid kitchenId, decimal minMarginPercent);
}
