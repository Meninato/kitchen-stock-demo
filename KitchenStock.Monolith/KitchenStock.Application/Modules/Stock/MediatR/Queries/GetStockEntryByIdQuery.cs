using KitchenStock.Application.Modules.Stock.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Stock.MediatR.Queries;

public record GetStockEntryByIdQuery(Guid StockId, Guid UserId) : IRequest<StockEntryResult>;