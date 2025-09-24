using KitchenStock.Application.Common.Pagination.Dtos;
using Microsoft.EntityFrameworkCore;

namespace KitchenStock.Infrastructure.Common.Pagination.Extensions;

public static class PaginationExtensions
{
    public static async Task<PaginatedResponseDto<T>> ToPaginatedResponseAsync<T>(
        this IQueryable<T> source,
        PaginationDto pagination,
        CancellationToken cancellationToken = default)
    {
        var totalItems = await source.CountAsync(cancellationToken);

        var items = await source
            .Skip(pagination.Skip)
            .Take(pagination.NormalizedPageSize)
            .ToListAsync(cancellationToken);

        return PaginatedResponseDto<T>.Create(
            items,
            pagination.NormalizedPage,
            pagination.NormalizedPageSize,
            totalItems
        );
    }
}
