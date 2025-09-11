using FluentResults;
using KitchenStock.Application.Modules.Ingredients.Dtos;

namespace KitchenStock.Application.Modules.Ingredients.Results;

public class IngredientListResult : Result<List<IngredientResponseDto>>
{
    public IngredientListResult() : base() { }

    protected IngredientListResult(List<IngredientResponseDto> value) : base()
    {
        WithValue(value);
    }

    protected IngredientListResult(IError error) : base()
    {
        WithError(error);
    }

    protected IngredientListResult(IEnumerable<IError> errors) : base()
    {
        WithErrors(errors);
    }

    public static IngredientListResult Success(List<IngredientResponseDto> kitchen) => new(kitchen);
    public static IngredientListResult Success() => new();
    public static IngredientListResult Failure(IError error) => new(error);
    public static IngredientListResult Failure(IEnumerable<IError> errors) => new(errors);
}
