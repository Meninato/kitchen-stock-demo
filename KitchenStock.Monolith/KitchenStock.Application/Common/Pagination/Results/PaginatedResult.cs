using FluentResults;
using KitchenStock.Application.Common.Pagination.Dtos;

namespace KitchenStock.Application.Common.Pagination.Results;

public class PaginatedResult<T> : Result<PaginatedResponseDto<T>>
{
    public PaginatedResult() : base() { }
    protected PaginatedResult(PaginatedResponseDto<T> value) : base() { WithValue(value); }
    protected PaginatedResult(IError error) : base() { WithError(error); }
    protected PaginatedResult(IEnumerable<IError> errors) : base() { WithErrors(errors); }

    public static PaginatedResult<T> Success(PaginatedResponseDto<T> response) => new(response);
    public static PaginatedResult<T> Failure(IError error) => new(error);
    public static PaginatedResult<T> Failure(IEnumerable<IError> errors) => new(errors);
}
