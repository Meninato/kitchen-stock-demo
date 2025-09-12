using KitchenStock.Application.Modules.Supplier.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Supplier.MediatR.Queries;

public record GetSupplierPerformanceQuery(Guid SupplierId, Guid UserId, DateTime? FromDate = null) : IRequest<SupplierPerformanceResult>;