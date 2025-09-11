using KitchenStock.Application.Modules.Ingredients.Abstractions;
using KitchenStock.Application.Modules.Ingredients.MediatR.Queries;
using KitchenStock.Application.Modules.Ingredients.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Ingredients.MediatR.Handlers;

public class GetLowStockIngredientsHandler : IRequestHandler<GetLowStockIngredientsQuery, IngredientListResult>
{
    private readonly IIngredientService _ingredientService;

    public GetLowStockIngredientsHandler(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    public async Task<IngredientListResult> Handle(GetLowStockIngredientsQuery request, CancellationToken cancellationToken)
    {
        return await _ingredientService.GetLowStockIngredientsAsync(request.KitchenId, request.UserId);
    }
}