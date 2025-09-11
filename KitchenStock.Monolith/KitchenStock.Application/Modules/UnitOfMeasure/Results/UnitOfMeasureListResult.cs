using FluentResults;
using KitchenStock.Application.Modules.UnitOfMeasure.Dtos;

namespace KitchenStock.Application.Modules.UnitOfMeasure.Results;

public class UnitOfMeasureListResult : Result<List<UnitOfMeasureResponseDto>>
{
    public UnitOfMeasureListResult() : base() { }
    protected UnitOfMeasureListResult(List<UnitOfMeasureResponseDto> value) : base() { WithValue(value); }
    protected UnitOfMeasureListResult(IError error) : base() { WithError(error); }
    protected UnitOfMeasureListResult(IEnumerable<IError> errors) : base() { WithErrors(errors); }

    public static UnitOfMeasureListResult Success(List<UnitOfMeasureResponseDto> units) => new(units);
    public static UnitOfMeasureListResult Failure(IError error) => new(error);
    public static UnitOfMeasureListResult Failure(IEnumerable<IError> errors) => new(errors);
}