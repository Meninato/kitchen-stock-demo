using KitchenStock.Application.Modules.Ingredients.Abstractions;
using KitchenStock.Application.Modules.Ingredients.MediatR.Commands;
using KitchenStock.Application.Modules.Ingredients.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Ingredients.MediatR.Handlers;

public class UpdateIngredientHandler : IRequestHandler<UpdateIngredientCommand, IngredientResult>
{
    private readonly IIngredientService _ingredientService;

    public UpdateIngredientHandler(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    public async Task<IngredientResult> Handle(UpdateIngredientCommand request, CancellationToken cancellationToken)
    {
        return await _ingredientService.UpdateIngredientAsync(request.IngredientId, request.UserId, request.Dto);
    }
}