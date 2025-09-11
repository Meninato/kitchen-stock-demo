using KitchenStock.Application.Modules.Stock.Abstractions;
using KitchenStock.Application.Modules.Stock.MediatR.Queries;
using KitchenStock.Application.Modules.Stock.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Stock.MediatR.Handlers;

public class GetKitchenStockHistoryHandler : IRequestHandler<GetKitchenStockHistoryQuery, StockEntryListResult>
{
    private readonly IStockEntryService _stockService;

    public GetKitchenStockHistoryHandler(IStockEntryService stockService)
    {
        _stockService = stockService;
    }

    public async Task<StockEntryListResult> Handle(GetKitchenStockHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _stockService.GetKitchenStockHistoryAsync(request.KitchenId, request.UserId, request.FromDate);
    }
}
