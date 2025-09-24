using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace KitchenStock.Api.Common.Pagination;

public record PaginationParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = null;
    public bool? SortDescending { get; set; } = false;
}