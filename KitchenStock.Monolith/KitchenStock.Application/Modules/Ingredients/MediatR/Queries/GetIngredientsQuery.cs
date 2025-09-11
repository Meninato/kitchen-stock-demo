using KitchenStock.Application.Modules.Ingredients.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Ingredients.MediatR.Queries;

public record GetIngredientsQuery(Guid KitchenId, Guid UserId) : IRequest<IngredientListResult>;