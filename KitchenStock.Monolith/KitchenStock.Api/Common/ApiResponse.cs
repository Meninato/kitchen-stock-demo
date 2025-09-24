using KitchenStock.Application.Common.Pagination.Dtos;

namespace KitchenStock.Api.Common;

public record ApiResponse<T>(T Data, PaginatedDetails? Pagination, string? Message);

public class ApiResponseBuilder<T>
{
    private T _data;
    private PaginatedDetails? _pagination;
    private string? _message;

    public ApiResponseBuilder(T data)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
    }

    public ApiResponseBuilder<T> WithPagination(PaginatedDetails pagination)
    {
        _pagination = pagination;
        return this;
    }

    public ApiResponseBuilder<T> WithMessage(string message)
    {
        _message = message;
        return this;
    }

    public ApiResponse<T> Build()
    {
        return new ApiResponse<T>(_data, _pagination, _message);
    }
}