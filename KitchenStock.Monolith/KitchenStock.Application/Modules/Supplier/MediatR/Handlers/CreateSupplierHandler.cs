using KitchenStock.Application.Modules.Supplier.Abstractions;
using KitchenStock.Application.Modules.Supplier.MediatR.Commands;
using KitchenStock.Application.Modules.Supplier.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Supplier.MediatR.Handlers;

public class CreateSupplierHandler : IRequestHandler<CreateSupplierCommand, SupplierResult>
{
    private readonly ISupplierService _supplierService;

    public CreateSupplierHandler(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    public async Task<SupplierResult> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        return await _supplierService.CreateSupplierAsync(request.UserId, request.Dto);
    }
}