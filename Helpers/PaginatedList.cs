using Microsoft.EntityFrameworkCore;

namespace dt191g_projekt.Helpers;

public class PaginatedList<T> : List<T>
{
    // Properties
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }

    public PaginatedList(List<T> posts, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        this.AddRange(posts);
    }

    // Properties to enable/disable previous and next buttons
    public bool HasPreviousPage => PageIndex > 1;

    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        var posts = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedList<T>(posts, count, pageIndex, pageSize);
    }
}