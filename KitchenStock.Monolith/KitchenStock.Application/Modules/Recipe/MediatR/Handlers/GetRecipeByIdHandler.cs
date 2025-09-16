using KitchenStock.Application.Modules.Recipe.Abstractions;
using KitchenStock.Application.Modules.Recipe.MediatR.Queries;
using KitchenStock.Application.Modules.Recipe.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Recipe.MediatR.Handlers;

public class GetRecipeByIdHandler : IRequestHandler<GetRecipeByIdQuery, RecipeResult>
{
    private readonly IRecipeService _recipeService;

    public GetRecipeByIdHandler(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public async Task<RecipeResult> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        return await _recipeService.GetRecipeByIdAsync(request.RecipeId, request.UserId);
    }
}