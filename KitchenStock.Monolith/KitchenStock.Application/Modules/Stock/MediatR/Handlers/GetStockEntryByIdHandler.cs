using KitchenStock.Application.Modules.Stock.Abstractions;
using KitchenStock.Application.Modules.Stock.MediatR.Queries;
using KitchenStock.Application.Modules.Stock.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Stock.MediatR.Handlers;

public class GetStockEntryByIdHandler : IRequestHandler<GetStockEntryByIdQuery, StockEntryResult>
{
    private readonly IStockEntryService _stockService;

    public GetStockEntryByIdHandler(IStockEntryService stockService)
    {
        _stockService = stockService;
    }

    public async Task<StockEntryResult> Handle(GetStockEntryByIdQuery request, CancellationToken cancellationToken)
    {
        return await _stockService.GetStockEntryByIdAsync(request.StockId, request.UserId);
    }
}