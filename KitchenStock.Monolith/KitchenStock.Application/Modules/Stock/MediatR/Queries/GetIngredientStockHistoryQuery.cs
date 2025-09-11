using KitchenStock.Application.Modules.Stock.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Stock.MediatR.Queries;

public record GetIngredientStockHistoryQuery(Guid IngredientId, Guid UserId) : IRequest<StockEntryListResult>;