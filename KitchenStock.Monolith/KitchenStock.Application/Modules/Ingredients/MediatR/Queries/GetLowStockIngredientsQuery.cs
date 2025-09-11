using KitchenStock.Application.Modules.Ingredients.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Ingredients.MediatR.Queries;

public record GetLowStockIngredientsQuery(Guid KitchenId, Guid UserId) : IRequest<IngredientListResult>;
