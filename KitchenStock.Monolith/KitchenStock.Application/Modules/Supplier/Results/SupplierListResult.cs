using FluentResults;
using KitchenStock.Application.Modules.Supplier.Dtos;

namespace KitchenStock.Application.Modules.Supplier.Results;

public class SupplierListResult : Result<List<SupplierResponseDto>>
{
    public SupplierListResult() : base() { }
    protected SupplierListResult(List<SupplierResponseDto> value) : base() { WithValue(value); }
    protected SupplierListResult(IError error) : base() { WithError(error); }
    protected SupplierListResult(IEnumerable<IError> errors) : base() { WithErrors(errors); }

    public static SupplierListResult Success(List<SupplierResponseDto> suppliers) => new(suppliers);
    public static SupplierListResult Failure(IError error) => new(error);
    public static SupplierListResult Failure(IEnumerable<IError> errors) => new(errors);
}