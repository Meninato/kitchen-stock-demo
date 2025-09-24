namespace KitchenStock.Application.Common.Pagination.Dtos;

public record PaginationDto(
    int Page = 1,
    int PageSize = 20,
    string? SortBy = null,
    bool SortDescending = false
)
{
    public int NormalizedPage => Math.Max(1, Page);
    public int NormalizedPageSize => Math.Max(1, Math.Min(100, PageSize)); // Cap at 100
    public int Skip => (NormalizedPage - 1) * NormalizedPageSize;

    public static PaginationDto Default => new();
}
