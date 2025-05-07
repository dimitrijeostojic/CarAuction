using SearchService.Models;
using SearchService.RequestHelpers;

namespace SearchService.Services.Interfaces
{
    public interface ISearchService
    {
        Task<(IReadOnlyList<Item> Results, long TotalCount, int PageCount)> SearchItemAsync(SearchParams searchParams);
    }
}
