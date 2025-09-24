using KitchenStock.Application.Modules.Ingredients.Abstractions;
using KitchenStock.Application.Modules.Ingredients.MediatR.Queries;
using KitchenStock.Application.Modules.Ingredients.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Ingredients.MediatR.Handlers;

public class GetIngredientsPagedHandler : IRequestHandler<GetIngredientsPagedQuery, IngredientPaginatedResult>
{
    private readonly IIngredientService _ingredientService;

    public GetIngredientsPagedHandler(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    public async Task<IngredientPaginatedResult> Handle(GetIngredientsPagedQuery request, CancellationToken cancellationToken)
    {
        return await _ingredientService.GetKitchenIngredientsPagedAsync(
            request.KitchenId, 
            request.UserId,
            request.Pagination,
            request.Filter);
    }
}