using KitchenStock.Application.Modules.Ingredients.Abstractions;
using KitchenStock.Application.Modules.Ingredients.MediatR.Queries;
using KitchenStock.Application.Modules.Ingredients.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Ingredients.MediatR.Handlers;

public class GetIngredientHandler : IRequestHandler<GetIngredientQuery, IngredientResult>
{
    private readonly IIngredientService _ingredientService;

    public GetIngredientHandler(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    public async Task<IngredientResult> Handle(GetIngredientQuery request, CancellationToken cancellationToken)
    {
        return await _ingredientService.GetIngredientByIdAsync(request.IngredientId, request.UserId);
    }
}
