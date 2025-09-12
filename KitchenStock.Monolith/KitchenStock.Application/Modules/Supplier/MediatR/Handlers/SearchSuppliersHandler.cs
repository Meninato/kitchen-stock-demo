using KitchenStock.Application.Modules.Supplier.Abstractions;
using KitchenStock.Application.Modules.Supplier.MediatR.Queries;
using KitchenStock.Application.Modules.Supplier.Results;
using MediatR;

namespace KitchenStock.Application.Modules.Supplier.MediatR.Handlers;

public class SearchSuppliersHandler : IRequestHandler<SearchSuppliersQuery, SupplierListResult>
{
    private readonly ISupplierService _supplierService;

    public SearchSuppliersHandler(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    public async Task<SupplierListResult> Handle(SearchSuppliersQuery request, CancellationToken cancellationToken)
    {
        return await _supplierService.SearchSuppliersAsync(request.SearchTerm, request.KitchenId, request.UserId);
    }
}