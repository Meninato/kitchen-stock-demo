using KitchenStock.Application.Modules.Supplier.Abstractions;
using KitchenStock.Application.Modules.Supplier.MediatR.Queries;
using KitchenStock.Application.Modules.Supplier.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Supplier.MediatR.Handlers;

public class GetSupplierByIdHandler : IRequestHandler<GetSupplierByIdQuery, SupplierResult>
{
    private readonly ISupplierService _supplierService;

    public GetSupplierByIdHandler(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    public async Task<SupplierResult> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        return await _supplierService.GetSupplierByIdAsync(request.SupplierId, request.UserId);
    }
}