namespace Domain.DTOs.Pagination;

public sealed class PaginatedResponse<T>
{
    public IReadOnlyList<T> Data { get; init; } = Array.Empty<T>();
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public bool HasPreviousPage { get; init; }
    public bool HasNextPage { get; init; }

    public static PaginatedResponse<T> Create(
        IReadOnlyList<T> data,
        int pageNumber,
        int pageSize,
        int totalCount)
    {
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        return new PaginatedResponse<T>
        {
            Data = data,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < totalPages
        };
    }
}
