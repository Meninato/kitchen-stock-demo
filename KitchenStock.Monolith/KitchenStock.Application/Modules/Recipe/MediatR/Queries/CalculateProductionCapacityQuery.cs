using KitchenStock.Application.Modules.Recipe.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Recipe.MediatR.Queries;

public record CalculateProductionCapacityQuery(Guid RecipeId, Guid UserId) : IRequest<RecipeProductionResult>;
