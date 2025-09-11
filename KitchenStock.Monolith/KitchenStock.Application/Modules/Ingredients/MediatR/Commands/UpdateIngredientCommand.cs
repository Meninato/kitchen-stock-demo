using KitchenStock.Application.Modules.Ingredients.Dtos;
using KitchenStock.Application.Modules.Ingredients.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Ingredients.MediatR.Commands;

public record UpdateIngredientCommand(Guid IngredientId, Guid UserId, UpdateIngredientDto Dto) : IRequest<IngredientResult>;