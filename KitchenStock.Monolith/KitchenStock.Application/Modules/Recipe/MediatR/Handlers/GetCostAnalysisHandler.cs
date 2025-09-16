using KitchenStock.Application.Modules.Recipe.Abstractions;
using KitchenStock.Application.Modules.Recipe.MediatR.Queries;
using KitchenStock.Application.Modules.Recipe.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Recipe.MediatR.Handlers;

public class GetCostAnalysisHandler : IRequestHandler<GetCostAnalysisQuery, RecipeCostAnalysisResult>
{
    private readonly IRecipeService _recipeService;

    public GetCostAnalysisHandler(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public async Task<RecipeCostAnalysisResult> Handle(GetCostAnalysisQuery request, CancellationToken cancellationToken)
    {
        return await _recipeService.GetCostAnalysisAsync(request.RecipeId, request.UserId);
    }
}
