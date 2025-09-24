using FluentResults;
using KitchenStock.Application.Common.Pagination.Dtos;
using KitchenStock.Application.Common.Pagination.Results;
using KitchenStock.Application.Modules.Ingredients.Dtos;

namespace KitchenStock.Application.Modules.Ingredients.Results;

public class IngredientPaginatedResult : Result<PaginatedResponseDto<IngredientResponseDto>>
{
    public IngredientPaginatedResult() : base() { }

    protected IngredientPaginatedResult(PaginatedResponseDto<IngredientResponseDto> value) : base()
    {
        WithValue(value);
    }

    protected IngredientPaginatedResult(IError error) : base()
    {
        WithError(error);
    }

    protected IngredientPaginatedResult(IEnumerable<IError> errors) : base()
    {
        WithErrors(errors);
    }

    public static IngredientPaginatedResult Success(PaginatedResponseDto<IngredientResponseDto> ingredient) => new(ingredient);
    public static IngredientPaginatedResult Success() => new();
    public static IngredientPaginatedResult Failure(IError error) => new(error);
    public static IngredientPaginatedResult Failure(IEnumerable<IError> errors) => new(errors);
}
