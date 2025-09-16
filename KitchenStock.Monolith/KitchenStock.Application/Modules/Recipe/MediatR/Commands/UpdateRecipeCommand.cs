using KitchenStock.Application.Modules.Recipe.Dtos;
using KitchenStock.Application.Modules.Recipe.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Recipe.MediatR.Commands;

public record UpdateRecipeCommand(Guid RecipeId, Guid UserId, UpdateRecipeDto Dto) : IRequest<RecipeResult>;