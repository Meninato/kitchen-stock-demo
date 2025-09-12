using FluentResults;
using KitchenStock.Application.Modules.Supplier.Dtos;

namespace KitchenStock.Application.Modules.Supplier.Results;

public class SupplierResult : Result<SupplierResponseDto>
{
    public SupplierResult() : base() { }
    protected SupplierResult(SupplierResponseDto value) : base() { WithValue(value); }
    protected SupplierResult(IError error) : base() { WithError(error); }
    protected SupplierResult(IEnumerable<IError> errors) : base() { WithErrors(errors); }

    public static SupplierResult Success(SupplierResponseDto supplier) => new(supplier);
    public static SupplierResult Success() => new();
    public static SupplierResult Failure(IError error) => new(error);
    public static SupplierResult Failure(IEnumerable<IError> errors) => new(errors);
}