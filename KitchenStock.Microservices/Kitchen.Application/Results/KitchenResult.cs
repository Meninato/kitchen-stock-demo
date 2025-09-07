using FluentResults;
using Kitchen.Application.Dtos;

namespace Kitchen.Application.Results;

public class KitchenResult : Result<KitchenResponseDto>
{
    public KitchenResult() : base() { }

    protected KitchenResult(KitchenResponseDto value) : base()
    {
        WithValue(value);
    }

    protected KitchenResult(IError error) : base()
    {
        WithError(error);
    }

    protected KitchenResult(IEnumerable<IError> errors) : base()
    {
        WithErrors(errors);
    }

    public static KitchenResult Success(KitchenResponseDto kitchen) => new(kitchen);
    public static KitchenResult Failure(IError error) => new(error);
    public static KitchenResult Failure(IEnumerable<IError> errors) => new(errors);
}
