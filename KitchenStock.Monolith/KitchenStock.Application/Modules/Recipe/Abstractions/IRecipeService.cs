using KitchenStock.Application.Modules.Recipe.Dtos;
using KitchenStock.Application.Modules.Recipe.Results;

namespace KitchenStock.Application.Modules.Recipe.Abstractions;

public interface IRecipeService
{
    Task<RecipeResult> CreateRecipeAsync(Guid userId, CreateRecipeDto request);
    Task<RecipeResult> GetRecipeByIdAsync(Guid id, Guid userId);
    Task<RecipeListResult> GetKitchenRecipesAsync(Guid kitchenId, Guid userId);
    Task<RecipeResult> UpdateRecipeAsync(Guid id, Guid userId, UpdateRecipeDto request);
    Task<RecipeResult> DeleteRecipeAsync(Guid id, Guid userId);
    Task<RecipeProductionResult> CalculateProductionCapacityAsync(Guid recipeId, Guid userId);
    Task<RecipeResult> ProduceRecipeAsync(Guid recipeId, Guid userId, ProduceRecipeDto request);
    Task<RecipeCostAnalysisResult> GetCostAnalysisAsync(Guid recipeId, Guid userId);
}