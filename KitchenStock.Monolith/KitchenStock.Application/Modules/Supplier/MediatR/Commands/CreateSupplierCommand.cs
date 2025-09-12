using KitchenStock.Application.Modules.Supplier.Dtos;
using KitchenStock.Application.Modules.Supplier.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Supplier.MediatR.Commands;

public record CreateSupplierCommand(Guid UserId, CreateSupplierDto Dto) : IRequest<SupplierResult>;