using KitchenStock.Application.Modules.Ingredients.Abstractions;
using KitchenStock.Application.Modules.Ingredients.MediatR.Queries;
using KitchenStock.Application.Modules.Ingredients.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Ingredients.MediatR.Handlers;

public class GetIngredientsHandler : IRequestHandler<GetIngredientsQuery, IngredientListResult>
{
    private readonly IIngredientService _ingredientService;

    public GetIngredientsHandler(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    public async Task<IngredientListResult> Handle(GetIngredientsQuery request, CancellationToken cancellationToken)
    {
        return await _ingredientService.GetKitchenIngredientsAsync(request.KitchenId, request.UserId);
    }
}
