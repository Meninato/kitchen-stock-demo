using KitchenStock.Application.Modules.Ingredients.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Ingredients.MediatR.Commands;

public record DeleteIngredientCommand(Guid IngredientId, Guid UserId) : IRequest<IngredientResult>;