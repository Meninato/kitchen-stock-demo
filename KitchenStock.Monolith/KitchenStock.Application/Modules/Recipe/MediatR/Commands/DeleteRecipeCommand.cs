using KitchenStock.Application.Modules.Recipe.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Recipe.MediatR.Commands;

public record DeleteRecipeCommand(Guid RecipeId, Guid UserId) : IRequest<RecipeResult>;