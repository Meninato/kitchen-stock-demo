using FluentResults;
using KitchenStock.Application.Modules.Stock.Dtos;

namespace KitchenStock.Application.Modules.Stock.Results;

public class StockEntryResult : Result<StockEntryResponseDto>
{
    public StockEntryResult() : base() { }
    protected StockEntryResult(StockEntryResponseDto value) : base() { WithValue(value); }
    protected StockEntryResult(IError error) : base() { WithError(error); }
    protected StockEntryResult(IEnumerable<IError> errors) : base() { WithErrors(errors); }

    public static StockEntryResult Success(StockEntryResponseDto stockEntry) => new(stockEntry);
    public static StockEntryResult Success() => new();
    public static StockEntryResult Failure(IError error) => new(error);
    public static StockEntryResult Failure(IEnumerable<IError> errors) => new(errors);
}
