using KitchenStock.Application.Modules.Stock.Dtos;
using KitchenStock.Application.Modules.Stock.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Stock.MediatR.Commands;

public record AddStockEntryCommand(Guid UserId, CreateStockEntryDto Dto) : IRequest<StockEntryResult>;
