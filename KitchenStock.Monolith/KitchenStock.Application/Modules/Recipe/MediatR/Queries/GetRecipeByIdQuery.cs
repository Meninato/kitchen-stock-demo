using KitchenStock.Application.Modules.Recipe.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Recipe.MediatR.Queries;

public record GetRecipeByIdQuery(Guid RecipeId, Guid UserId) : IRequest<RecipeResult>;
