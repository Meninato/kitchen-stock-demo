using KitchenStock.Application.Ingredient.Dto;

namespace KitchenStock.Application.Abstractions;

public interface IIngredientService
{
    Task<IngredientResult> CreateIngredientAsync(Guid userId, CreateIngredientDto request);
    Task<IngredientResult> GetIngredientByIdAsync(Guid id, Guid userId);
    Task<IngredientListResult> GetKitchenIngredientsAsync(Guid kitchenId, Guid userId);
    Task<IngredientListResult> GetLowStockIngredientsAsync(Guid kitchenId, Guid userId);
    Task<IngredientResult> UpdateIngredientAsync(Guid id, Guid userId, UpdateIngredientRequest request);
    Task<IngredientResult> DeleteIngredientAsync(Guid id, Guid userId);
    Task<IngredientResult> UpdateStockAsync(Guid id, Guid userId, UpdateStockRequest request);
}
