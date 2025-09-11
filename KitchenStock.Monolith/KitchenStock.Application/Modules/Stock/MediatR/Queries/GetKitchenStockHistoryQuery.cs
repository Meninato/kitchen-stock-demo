using KitchenStock.Application.Modules.Stock.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Stock.MediatR.Queries;

public record GetKitchenStockHistoryQuery(Guid KitchenId, Guid UserId, DateTime? FromDate = null) : IRequest<StockEntryListResult>;