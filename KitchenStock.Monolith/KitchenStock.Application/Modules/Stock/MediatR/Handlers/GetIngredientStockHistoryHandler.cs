using KitchenStock.Application.Modules.Stock.Abstractions;
using KitchenStock.Application.Modules.Stock.MediatR.Queries;
using KitchenStock.Application.Modules.Stock.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Stock.MediatR.Handlers;

public class GetIngredientStockHistoryHandler : IRequestHandler<GetIngredientStockHistoryQuery, StockEntryListResult>
{
    private readonly IStockEntryService _stockService;

    public GetIngredientStockHistoryHandler(IStockEntryService stockService)
    {
        _stockService = stockService;
    }

    public async Task<StockEntryListResult> Handle(GetIngredientStockHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _stockService.GetIngredientStockHistoryAsync(request.IngredientId, request.UserId);
    }
}