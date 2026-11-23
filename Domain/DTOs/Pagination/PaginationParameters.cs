namespace Domain.DTOs.Pagination;

public sealed class PaginationParameters
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 10;
    public const int MaximumPageSize = 50;

    public int PageNumber { get; set; } = DefaultPageNumber;
    public int PageSize { get; set; } = DefaultPageSize;

    public PaginationParameters Normalize()
    {
        return new PaginationParameters
        {
            PageNumber = PageNumber < 1 ? DefaultPageNumber : PageNumber,
            PageSize = PageSize < 1
                ? DefaultPageSize
                : Math.Min(PageSize, MaximumPageSize)
        };
    }
}
