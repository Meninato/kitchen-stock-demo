using FluentResults;
using KitchenStock.Application.Kitchen.Dtos;

namespace KitchenStock.Application.Kitchen.Results;

public class KitchenListResult : Result<List<KitchenResponseDto>>
{
    public KitchenListResult() : base() { }

    protected KitchenListResult(List<KitchenResponseDto> value) : base()
    {
        WithValue(value);
    }

    protected KitchenListResult(IError error) : base()
    {
        WithError(error);
    }

    protected KitchenListResult(IEnumerable<IError> errors) : base()
    {
        WithErrors(errors);
    }

    public static KitchenListResult Success(List<KitchenResponseDto> kitchen) => new(kitchen);
    public static KitchenListResult Failure(IError error) => new(error);
    public static KitchenListResult Failure(IEnumerable<IError> errors) => new(errors);
}
