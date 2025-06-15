using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;
using SearchService.Services.Interfaces;
using System.Diagnostics;

namespace SearchService.Services.Implementation
{
    public class SearchItems : ISearchService
    {
        public async Task<(IReadOnlyList<Item> Results, long TotalCount, int PageCount)> SearchItemAsync(SearchParams searchParams)
        {
            var query = DB.PagedSearch<Item, Item>();

            query = ApplyFiltering(query, searchParams);
            query = ApplyOrdering(query, searchParams.OrderBy);

            // Pagination
            query.PageNumber(searchParams.PageNumber > 0 ? searchParams.PageNumber : 1);
            query.PageSize(searchParams.PageSize > 0 ? searchParams.PageSize : 10);

            Console.WriteLine($"PageNumber: {searchParams.PageNumber}, PageSize: {searchParams.PageSize}");
            Debug.WriteLine($"PageNumber: {searchParams.PageNumber}, PageSize: {searchParams.PageSize}");

            var result = await query.ExecuteAsync();
            return result;
        }

        private PagedSearch<Item, Item> ApplyFiltering(PagedSearch<Item, Item> query, SearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
            {
                query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore(); // Override sort only if search term is present
            }

            if (!string.IsNullOrEmpty(searchParams.Seller))
                query.Match(x => x.Seller == searchParams.Seller);

            if (!string.IsNullOrEmpty(searchParams.Winner))
                query.Match(x => x.Winner == searchParams.Winner);

            query = searchParams.FilterBy switch
            {
                "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
                "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) && x.AuctionEnd > DateTime.UtcNow),
                _ => query.Match(x=>true)
            };

            return query;
        }

        private PagedSearch<Item, Item> ApplyOrdering(PagedSearch<Item, Item> query, string? orderBy)
        {
            // Ako je već SortByTextScore pozvan ranije, dodatni sort neće imati efekta osim ako nije poništen
            return orderBy switch
            {
                "make" => query.Sort(x => x.Ascending(a => a.Make)),
                "new" => query.Sort(x => x.Descending(a => a.CreatedAt)),
                _ => query.Sort(x => x.Ascending(a => a.AuctionEnd))
            };
        }
    }
}
