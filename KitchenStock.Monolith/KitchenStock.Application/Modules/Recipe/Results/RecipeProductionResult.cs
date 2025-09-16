using FluentResults;
using KitchenStock.Application.Modules.Recipe.Dtos;

namespace KitchenStock.Application.Modules.Recipe.Results;

public class RecipeProductionResult : Result<RecipeProductionResponseDto>
{
    public RecipeProductionResult() : base() { }
    protected RecipeProductionResult(RecipeProductionResponseDto value) : base() { WithValue(value); }
    protected RecipeProductionResult(IError error) : base() { WithError(error); }
    protected RecipeProductionResult(IEnumerable<IError> errors) : base() { WithErrors(errors); }

    public static RecipeProductionResult Success(RecipeProductionResponseDto production) => new(production);
    public static RecipeProductionResult Failure(IError error) => new(error);
    public static RecipeProductionResult Failure(IEnumerable<IError> errors) => new(errors);
}
