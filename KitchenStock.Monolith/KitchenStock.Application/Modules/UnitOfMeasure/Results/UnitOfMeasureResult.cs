using FluentResults;
using KitchenStock.Application.Modules.UnitOfMeasure.Dtos;

namespace KitchenStock.Application.Modules.UnitOfMeasure.Results;

public class UnitOfMeasureResult : Result<UnitOfMeasureResponseDto>
{
    public UnitOfMeasureResult() : base() { }
    protected UnitOfMeasureResult(UnitOfMeasureResponseDto value) : base() { WithValue(value); }
    protected UnitOfMeasureResult(IError error) : base() { WithError(error); }
    protected UnitOfMeasureResult(IEnumerable<IError> errors) : base() { WithErrors(errors); }

    public static UnitOfMeasureResult Success(UnitOfMeasureResponseDto unit) => new(unit);
    public static UnitOfMeasureResult Success() => new();
    public static UnitOfMeasureResult Failure(IError error) => new(error);
    public static UnitOfMeasureResult Failure(IEnumerable<IError> errors) => new(errors);
}