using KitchenStock.Application.Modules.Recipe.Abstractions;
using KitchenStock.Application.Modules.Recipe.MediatR.Commands;
using KitchenStock.Application.Modules.Recipe.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Recipe.MediatR.Handlers;

public class UpdateRecipeHandler : IRequestHandler<UpdateRecipeCommand, RecipeResult>
{
    private readonly IRecipeService _recipeService;

    public UpdateRecipeHandler(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public async Task<RecipeResult> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        return await _recipeService.UpdateRecipeAsync(request.RecipeId, request.UserId, request.Dto);
    }
}