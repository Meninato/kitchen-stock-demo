using KitchenStock.Application.Modules.Stock.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Stock.MediatR.Queries;

public record GetStockSummaryQuery(Guid KitchenId, Guid UserId) : IRequest<StockSummaryResult>;
