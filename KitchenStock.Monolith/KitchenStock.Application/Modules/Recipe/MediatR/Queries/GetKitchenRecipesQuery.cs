using KitchenStock.Application.Modules.Recipe.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Recipe.MediatR.Queries;

public record GetKitchenRecipesQuery(Guid KitchenId, Guid UserId) : IRequest<RecipeListResult>;
