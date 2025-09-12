using KitchenStock.Application.Modules.Supplier.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Supplier.MediatR.Queries;

public record GetKitchenSuppliersQuery(Guid KitchenId, Guid UserId) : IRequest<SupplierListResult>;