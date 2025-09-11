using KitchenStock.Application.Modules.Stock.Abstractions;
using KitchenStock.Application.Modules.Stock.MediatR.Commands;
using KitchenStock.Application.Modules.Stock.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Stock.MediatR.Handlers;

public class AddStockEntryHandler : IRequestHandler<AddStockEntryCommand, StockEntryResult>
{
    private readonly IStockEntryService _stockService;

    public AddStockEntryHandler(IStockEntryService stockService)
    {
        _stockService = stockService;
    }

    public async Task<StockEntryResult> Handle(AddStockEntryCommand request, CancellationToken cancellationToken)
    {
        return await _stockService.AddStockEntryAsync(request.UserId, request.Dto);
    }
}