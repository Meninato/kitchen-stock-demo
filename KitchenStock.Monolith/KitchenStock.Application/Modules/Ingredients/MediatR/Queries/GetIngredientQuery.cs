using KitchenStock.Application.Modules.Ingredients.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Ingredients.MediatR.Queries;

public record GetIngredientQuery(Guid IngredientId, Guid UserId) : IRequest<IngredientResult>;