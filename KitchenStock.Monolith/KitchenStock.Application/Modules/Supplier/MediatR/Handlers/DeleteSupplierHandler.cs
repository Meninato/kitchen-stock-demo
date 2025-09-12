using KitchenStock.Application.Modules.Supplier.Abstractions;
using KitchenStock.Application.Modules.Supplier.MediatR.Commands;
using KitchenStock.Application.Modules.Supplier.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Supplier.MediatR.Handlers;

public class DeleteSupplierHandler : IRequestHandler<DeleteSupplierCommand, SupplierResult>
{
    private readonly ISupplierService _supplierService;

    public DeleteSupplierHandler(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    public async Task<SupplierResult> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        return await _supplierService.DeleteSupplierAsync(request.SupplierId, request.UserId);
    }
}
