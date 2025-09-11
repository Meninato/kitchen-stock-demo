using FluentResults;
using KitchenStock.Application.Modules.Ingredients.Dtos;

namespace KitchenStock.Application.Modules.Ingredients.Results;

public class IngredientResult : Result<IngredientResponseDto>
{
    public IngredientResult() : base() { }

    protected IngredientResult(IngredientResponseDto value) : base()
    {
        WithValue(value);
    }

    protected IngredientResult(IError error) : base()
    {
        WithError(error);
    }

    protected IngredientResult(IEnumerable<IError> errors) : base()
    {
        WithErrors(errors);
    }

    public static IngredientResult Success(IngredientResponseDto kitchen) => new(kitchen);
    public static IngredientResult Success() => new();
    public static IngredientResult Failure(IError error) => new(error);
    public static IngredientResult Failure(IEnumerable<IError> errors) => new(errors);
}
