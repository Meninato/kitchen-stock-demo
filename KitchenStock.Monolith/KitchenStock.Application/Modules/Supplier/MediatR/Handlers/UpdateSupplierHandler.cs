using KitchenStock.Application.Modules.Supplier.Abstractions;
using KitchenStock.Application.Modules.Supplier.MediatR.Commands;
using KitchenStock.Application.Modules.Supplier.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Supplier.MediatR.Handlers;

public class UpdateSupplierHandler : IRequestHandler<UpdateSupplierCommand, SupplierResult>
{
    private readonly ISupplierService _supplierService;

    public UpdateSupplierHandler(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    public async Task<SupplierResult> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        return await _supplierService.UpdateSupplierAsync(request.SupplierId, request.UserId, request.Dto);
    }
}