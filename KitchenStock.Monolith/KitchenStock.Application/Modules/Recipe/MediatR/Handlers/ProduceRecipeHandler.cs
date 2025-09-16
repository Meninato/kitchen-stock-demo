using KitchenStock.Application.Modules.Recipe.Abstractions;
using KitchenStock.Application.Modules.Recipe.MediatR.Commands;
using KitchenStock.Application.Modules.Recipe.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Recipe.MediatR.Handlers;

public class ProduceRecipeHandler : IRequestHandler<ProduceRecipeCommand, RecipeResult>
{
    private readonly IRecipeService _recipeService;

    public ProduceRecipeHandler(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public async Task<RecipeResult> Handle(ProduceRecipeCommand request, CancellationToken cancellationToken)
    {
        return await _recipeService.ProduceRecipeAsync(request.RecipeId, request.UserId, request.Dto);
    }
}
