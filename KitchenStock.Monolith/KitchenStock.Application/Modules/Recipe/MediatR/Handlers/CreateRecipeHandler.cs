using KitchenStock.Application.Modules.Recipe.Abstractions;
using KitchenStock.Application.Modules.Recipe.MediatR.Commands;
using KitchenStock.Application.Modules.Recipe.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Recipe.MediatR.Handlers;

public class CreateRecipeHandler : IRequestHandler<CreateRecipeCommand, RecipeResult>
{
    private readonly IRecipeService _recipeService;

    public CreateRecipeHandler(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public async Task<RecipeResult> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        return await _recipeService.CreateRecipeAsync(request.UserId, request.Dto);
    }
}