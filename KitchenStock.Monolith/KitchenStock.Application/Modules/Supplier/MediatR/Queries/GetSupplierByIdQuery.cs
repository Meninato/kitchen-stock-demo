using KitchenStock.Application.Modules.Supplier.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Supplier.MediatR.Queries;

public record GetSupplierByIdQuery(Guid SupplierId, Guid UserId) : IRequest<SupplierResult>;