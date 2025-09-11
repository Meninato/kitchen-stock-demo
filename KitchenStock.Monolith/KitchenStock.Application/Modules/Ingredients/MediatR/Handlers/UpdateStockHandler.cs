using KitchenStock.Application.Modules.Ingredients.Abstractions;
using KitchenStock.Application.Modules.Ingredients.MediatR.Commands;
using KitchenStock.Application.Modules.Ingredients.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Ingredients.MediatR.Handlers;

public class UpdateStockHandler : IRequestHandler<UpdateStockCommand, IngredientResult>
{
    private readonly IIngredientService _ingredientService;

    public UpdateStockHandler(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    public async Task<IngredientResult> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        return await _ingredientService.UpdateStockAsync(request.IngredientId, request.UserId, request.Dto);
    }
}
