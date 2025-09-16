using FluentResults;
using KitchenStock.Application.Modules.Recipe.Dtos;

namespace KitchenStock.Application.Modules.Recipe.Results;

public class RecipeListResult : Result<List<RecipeResponseDto>>
{
    public RecipeListResult() : base() { }
    protected RecipeListResult(List<RecipeResponseDto> value) : base() { WithValue(value); }
    protected RecipeListResult(IError error) : base() { WithError(error); }
    protected RecipeListResult(IEnumerable<IError> errors) : base() { WithErrors(errors); }

    public static RecipeListResult Success(List<RecipeResponseDto> recipes) => new(recipes);
    public static RecipeListResult Failure(IError error) => new(error);
    public static RecipeListResult Failure(IEnumerable<IError> errors) => new(errors);
}
