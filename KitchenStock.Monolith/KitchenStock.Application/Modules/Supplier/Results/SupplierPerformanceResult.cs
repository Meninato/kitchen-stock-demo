using FluentResults;
using KitchenStock.Application.Modules.Supplier.Dtos;

namespace KitchenStock.Application.Modules.Supplier.Results;

public class SupplierPerformanceResult : Result<SupplierPerformanceResponseDto>
{
    public SupplierPerformanceResult() : base() { }
    protected SupplierPerformanceResult(SupplierPerformanceResponseDto value) : base() { WithValue(value); }
    protected SupplierPerformanceResult(IError error) : base() { WithError(error); }
    protected SupplierPerformanceResult(IEnumerable<IError> errors) : base() { WithErrors(errors); }

    public static SupplierPerformanceResult Success(SupplierPerformanceResponseDto performance) => new(performance);
    public static SupplierPerformanceResult Failure(IError error) => new(error);
    public static SupplierPerformanceResult Failure(IEnumerable<IError> errors) => new(errors);
}