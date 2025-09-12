using KitchenStock.Application.Modules.Supplier.Dtos;
using KitchenStock.Application.Modules.Supplier.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Supplier.MediatR.Commands;

public record UpdateSupplierCommand(Guid SupplierId, Guid UserId, UpdateSupplierDto Dto) : IRequest<SupplierResult>;