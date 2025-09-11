using FluentResults;
using KitchenStock.Application.Modules.Stock.Dtos;

namespace KitchenStock.Application.Modules.Stock.Results;
public class StockSummaryResult : Result<StockSummaryResponseDto>
{
    public StockSummaryResult() : base() { }
    protected StockSummaryResult(StockSummaryResponseDto value) : base() { WithValue(value); }
    protected StockSummaryResult(IError error) : base() { WithError(error); }
    protected StockSummaryResult(IEnumerable<IError> errors) : base() { WithErrors(errors); }

    public static StockSummaryResult Success(StockSummaryResponseDto summary) => new(summary);
    public static StockSummaryResult Failure(IError error) => new(error);
    public static StockSummaryResult Failure(IEnumerable<IError> errors) => new(errors);
}
