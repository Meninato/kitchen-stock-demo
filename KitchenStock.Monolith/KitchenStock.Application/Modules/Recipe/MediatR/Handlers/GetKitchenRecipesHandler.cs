using KitchenStock.Application.Modules.Recipe.Abstractions;
using KitchenStock.Application.Modules.Recipe.MediatR.Queries;
using KitchenStock.Application.Modules.Recipe.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Recipe.MediatR.Handlers;

public class GetKitchenRecipesHandler : IRequestHandler<GetKitchenRecipesQuery, RecipeListResult>
{
    private readonly IRecipeService _recipeService;

    public GetKitchenRecipesHandler(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public async Task<RecipeListResult> Handle(GetKitchenRecipesQuery request, CancellationToken cancellationToken)
    {
        return await _recipeService.GetKitchenRecipesAsync(request.KitchenId, request.UserId);
    }
}
