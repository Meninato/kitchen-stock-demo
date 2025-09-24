using KitchenStock.Application.Common.Pagination.Dtos;
using KitchenStock.Application.Common.Pagination.Results;
using KitchenStock.Application.Modules.Ingredients.Dtos;
using KitchenStock.Application.Modules.Ingredients.Results;

namespace KitchenStock.Application.Modules.Ingredients.Abstractions;

public interface IIngredientService
{
    Task<IngredientResult> CreateIngredientAsync(Guid userId, CreateIngredientDto request);
    Task<IngredientResult> GetIngredientByIdAsync(Guid id, Guid userId);
    Task<IngredientListResult> GetKitchenIngredientsAsync(Guid kitchenId, Guid userId);
    Task<IngredientListResult> GetLowStockIngredientsAsync(Guid kitchenId, Guid userId);
    Task<IngredientResult> UpdateIngredientAsync(Guid id, Guid userId, UpdateIngredientDto request);
    Task<IngredientResult> DeleteIngredientAsync(Guid id, Guid userId);
    Task<IngredientResult> UpdateStockAsync(Guid id, Guid userId, UpdateStockDto request);

    Task<IngredientPaginatedResult> GetKitchenIngredientsPagedAsync(
        Guid kitchenId,
        Guid userId,
        PaginationDto? pagination = null,
        IngredientFilterDto? filter = null);
}
