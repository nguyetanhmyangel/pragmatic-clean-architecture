namespace Bookify.ShareKernel.Utilities;

public interface IPagedResult<T>
{
    IReadOnlyList<T> Items { get; }
    int TotalCount { get; }
    int PageIndex { get; }
    int PageSize { get; }
}