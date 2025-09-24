using KitchenStock.Application.Common.Pagination.Dtos;
using KitchenStock.Application.Modules.Ingredients.Dtos;
using KitchenStock.Application.Modules.Ingredients.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Ingredients.MediatR.Queries;

public record GetIngredientsPagedQuery(
    Guid KitchenId, 
    Guid UserId,
    PaginationDto? Pagination = null,
    IngredientFilterDto? Filter = null
) : IRequest<IngredientPaginatedResult>;