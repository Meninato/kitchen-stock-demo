using KitchenStock.Application.Modules.Supplier.Abstractions;
using KitchenStock.Application.Modules.Supplier.MediatR.Queries;
using KitchenStock.Application.Modules.Supplier.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Supplier.MediatR.Handlers;

public class GetSupplierPerformanceHandler : IRequestHandler<GetSupplierPerformanceQuery, SupplierPerformanceResult>
{
    private readonly ISupplierService _supplierService;

    public GetSupplierPerformanceHandler(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    public async Task<SupplierPerformanceResult> Handle(GetSupplierPerformanceQuery request, CancellationToken cancellationToken)
    {
        return await _supplierService.GetSupplierPerformanceAsync(request.SupplierId, request.UserId, request.FromDate);
    }
}