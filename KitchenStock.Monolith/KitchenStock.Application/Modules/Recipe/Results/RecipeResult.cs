using FluentResults;
using KitchenStock.Application.Modules.Recipe.Dtos;

namespace KitchenStock.Application.Modules.Recipe.Results;

public class RecipeResult : Result<RecipeResponseDto>
{
    public RecipeResult() : base() { }
    protected RecipeResult(RecipeResponseDto value) : base() { WithValue(value); }
    protected RecipeResult(IError error) : base() { WithError(error); }
    protected RecipeResult(IEnumerable<IError> errors) : base() { WithErrors(errors); }

    public static RecipeResult Success(RecipeResponseDto recipe) => new(recipe);
    public static RecipeResult Success() => new();
    public static RecipeResult Failure(IError error) => new(error);
    public static RecipeResult Failure(IEnumerable<IError> errors) => new(errors);
}