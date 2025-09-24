namespace KitchenStock.Application.Common.Pagination.Dtos;

public record PaginatedResponseDto<T>(
    List<T> Items,
    PaginatedDetails Details
)
{
    public static PaginatedResponseDto<T> Create(List<T> items, int page, int pageSize, int totalItems)
    {
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        return new PaginatedResponseDto<T>(
            Items: items,
            Details: new PaginatedDetails(
                Page: page,
                PageSize: pageSize,
                TotalItems: totalItems,
                TotalPages: totalPages,
                HasPrevious: page > 1,
                HasNext: page < totalPages
            )
        );
    }
}

public record PaginatedDetails(int Page, int PageSize, int TotalItems, int TotalPages, bool HasPrevious, bool HasNext);
