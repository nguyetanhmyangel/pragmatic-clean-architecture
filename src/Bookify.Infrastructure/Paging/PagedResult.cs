using Bookify.ShareKernel.Utilities;

namespace Bookify.Infrastructure.Paging;

public class PagedResult<T> : IPagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = new List<T>();
    public int TotalCount { get; init; }
    public int PageIndex { get; init; }
    public int PageSize { get; init; }

    public PagedResult() { }

    public PagedResult(IReadOnlyList<T> items, int totalCount, int pageIndex, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }
}