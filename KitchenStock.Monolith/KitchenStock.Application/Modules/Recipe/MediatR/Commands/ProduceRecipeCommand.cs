using KitchenStock.Application.Modules.Recipe.Dtos;
using KitchenStock.Application.Modules.Recipe.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Recipe.MediatR.Commands;

public record ProduceRecipeCommand(Guid RecipeId, Guid UserId, ProduceRecipeDto Dto) : IRequest<RecipeResult>;