using KitchenStock.Application.Modules.Ingredients.Abstractions;
using KitchenStock.Application.Modules.Ingredients.MediatR.Commands;
using KitchenStock.Application.Modules.Ingredients.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Ingredients.MediatR.Handlers;

public class DeleteIngredientHandler : IRequestHandler<DeleteIngredientCommand, IngredientResult>
{
    private readonly IIngredientService _ingredientService;

    public DeleteIngredientHandler(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    public async Task<IngredientResult> Handle(DeleteIngredientCommand request, CancellationToken cancellationToken)
    {
        return await _ingredientService.DeleteIngredientAsync(request.IngredientId, request.UserId);
    }
}