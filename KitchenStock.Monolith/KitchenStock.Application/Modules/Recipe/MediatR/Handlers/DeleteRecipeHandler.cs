using KitchenStock.Application.Modules.Recipe.Abstractions;
using KitchenStock.Application.Modules.Recipe.MediatR.Commands;
using KitchenStock.Application.Modules.Recipe.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Recipe.MediatR.Handlers;

public class DeleteRecipeHandler : IRequestHandler<DeleteRecipeCommand, RecipeResult>
{
    private readonly IRecipeService _recipeService;

    public DeleteRecipeHandler(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public async Task<RecipeResult> Handle(DeleteRecipeCommand request, CancellationToken cancellationToken)
    {
        return await _recipeService.DeleteRecipeAsync(request.RecipeId, request.UserId);
    }
}
