using KitchenStock.Application.Modules.Stock.Abstractions;
using KitchenStock.Application.Modules.Stock.MediatR.Queries;
using KitchenStock.Application.Modules.Stock.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Stock.MediatR.Handlers;

public class GetStockSummaryHandler : IRequestHandler<GetStockSummaryQuery, StockSummaryResult>
{
    private readonly IStockEntryService _stockService;

    public GetStockSummaryHandler(IStockEntryService stockService)
    {
        _stockService = stockService;
    }

    public async Task<StockSummaryResult> Handle(GetStockSummaryQuery request, CancellationToken cancellationToken)
    {
        return await _stockService.GetStockSummaryAsync(request.KitchenId, request.UserId);
    }
}
