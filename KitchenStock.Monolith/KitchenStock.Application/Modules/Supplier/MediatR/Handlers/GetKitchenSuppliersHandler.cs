using KitchenStock.Application.Modules.Supplier.Abstractions;
using KitchenStock.Application.Modules.Supplier.MediatR.Queries;
using KitchenStock.Application.Modules.Supplier.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Supplier.MediatR.Handlers;

public class GetKitchenSuppliersHandler : IRequestHandler<GetKitchenSuppliersQuery, SupplierListResult>
{
    private readonly ISupplierService _supplierService;

    public GetKitchenSuppliersHandler(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    public async Task<SupplierListResult> Handle(GetKitchenSuppliersQuery request, CancellationToken cancellationToken)
    {
        return await _supplierService.GetKitchenSuppliersAsync(request.KitchenId, request.UserId);
    }
}