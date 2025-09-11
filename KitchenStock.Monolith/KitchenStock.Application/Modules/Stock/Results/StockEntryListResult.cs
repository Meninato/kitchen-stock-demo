using FluentResults;
using KitchenStock.Application.Modules.Stock.Dtos;

namespace KitchenStock.Application.Modules.Stock.Results;

public class StockEntryListResult : Result<List<StockEntryResponseDto>>
{
    public StockEntryListResult() : base() { }
    protected StockEntryListResult(List<StockEntryResponseDto> value) : base() { WithValue(value); }
    protected StockEntryListResult(IError error) : base() { WithError(error); }
    protected StockEntryListResult(IEnumerable<IError> errors) : base() { WithErrors(errors); }

    public static StockEntryListResult Success(List<StockEntryResponseDto> entries) => new(entries);
    public static StockEntryListResult Failure(IError error) => new(error);
    public static StockEntryListResult Failure(IEnumerable<IError> errors) => new(errors);
}