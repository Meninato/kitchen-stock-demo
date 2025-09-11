using KitchenStock.Application.Modules.Ingredients.Abstractions;
using KitchenStock.Application.Modules.Ingredients.MediatR.Commands;
using KitchenStock.Application.Modules.Ingredients.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Ingredients.MediatR.Handlers;

public class CreateIngredientHandler : IRequestHandler<CreateIngredientCommand, IngredientResult>
{
    private readonly IIngredientService _ingredientService;

    public CreateIngredientHandler(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    public async Task<IngredientResult> Handle(CreateIngredientCommand request, CancellationToken cancellationToken)
    {
        return await _ingredientService.CreateIngredientAsync(request.UserId, request.Dto);
    }
}
