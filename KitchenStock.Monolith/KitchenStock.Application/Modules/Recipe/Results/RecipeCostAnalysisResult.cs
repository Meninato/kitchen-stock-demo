using FluentResults;
using KitchenStock.Application.Modules.Recipe.Dtos;

namespace KitchenStock.Application.Modules.Recipe.Results;

public class RecipeCostAnalysisResult : Result<RecipeCostAnalysisResponseDto>
{
    public RecipeCostAnalysisResult() : base() { }
    protected RecipeCostAnalysisResult(RecipeCostAnalysisResponseDto value) : base() { WithValue(value); }
    protected RecipeCostAnalysisResult(IError error) : base() { WithError(error); }
    protected RecipeCostAnalysisResult(IEnumerable<IError> errors) : base() { WithErrors(errors); }

    public static RecipeCostAnalysisResult Success(RecipeCostAnalysisResponseDto analysis) => new(analysis);
    public static RecipeCostAnalysisResult Failure(IError error) => new(error);
    public static RecipeCostAnalysisResult Failure(IEnumerable<IError> errors) => new(errors);
}