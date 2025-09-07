using FluentResults;
using Kitchen.Application.Dtos;

namespace Kitchen.Application.Results;

public class KitchenListResult : Result<IEnumerable<KitchenResponseDto>>
{
    public KitchenListResult() : base() { }

    protected KitchenListResult(IEnumerable<KitchenResponseDto> value) : base()
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

    public static KitchenListResult Success(IEnumerable<KitchenResponseDto> kitchens) => new(kitchens);
    public static KitchenListResult Failure(IError error) => new(error);
    public static KitchenListResult Failure(IEnumerable<IError> errors) => new(errors);
}
