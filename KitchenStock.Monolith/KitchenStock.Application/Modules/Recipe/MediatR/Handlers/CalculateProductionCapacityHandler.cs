using KitchenStock.Application.Modules.Recipe.Abstractions;
using KitchenStock.Application.Modules.Recipe.MediatR.Queries;
using KitchenStock.Application.Modules.Recipe.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Recipe.MediatR.Handlers;

public class CalculateProductionCapacityHandler : IRequestHandler<CalculateProductionCapacityQuery, RecipeProductionResult>
{
    private readonly IRecipeService _recipeService;

    public CalculateProductionCapacityHandler(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public async Task<RecipeProductionResult> Handle(CalculateProductionCapacityQuery request, CancellationToken cancellationToken)
    {
        return await _recipeService.CalculateProductionCapacityAsync(request.RecipeId, request.UserId);
    }
}
