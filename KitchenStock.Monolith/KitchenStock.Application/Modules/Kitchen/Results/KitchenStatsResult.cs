using FluentResults;
using KitchenStock.Application.Modules.Kitchen.Dtos;

namespace KitchenStock.Application.Modules.Kitchen.Results;

public class KitchenStatsResult : Result<KitchenStatsResponseDto>
{
    public KitchenStatsResult() : base() { }

    protected KitchenStatsResult(KitchenStatsResponseDto value) : base()
    {
        WithValue(value);
    }

    protected KitchenStatsResult(IError error) : base()
    {
        WithError(error);
    }

    protected KitchenStatsResult(IEnumerable<IError> errors) : base()
    {
        WithErrors(errors);
    }

    public static KitchenStatsResult Success(KitchenStatsResponseDto kitchen) => new(kitchen);
    public static KitchenStatsResult Failure(IError error) => new(error);
    public static KitchenStatsResult Failure(IEnumerable<IError> errors) => new(errors);
}