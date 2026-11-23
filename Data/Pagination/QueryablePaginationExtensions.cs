using Domain.DTOs.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Data.Pagination;

public static class QueryablePaginationExtensions
{
    public static async Task<PaginatedResponse<T>> ToPaginatedResponseAsync<T>(
        this IQueryable<T> query,
        PaginationParameters pagination,
        CancellationToken cancellationToken = default)
    {
        var normalized = pagination.Normalize();
        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (int)Math.Min((long)(normalized.PageNumber - 1) * normalized.PageSize, int.MaxValue);
        var data = await query
            .Skip(skip)
            .Take(normalized.PageSize)
            .ToListAsync(cancellationToken);

        return PaginatedResponse<T>.Create(data, normalized.PageNumber, normalized.PageSize, totalCount);
    }
}
