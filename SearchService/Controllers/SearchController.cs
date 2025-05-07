using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;
using SearchService.Services.Interfaces;

namespace SearchService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService searchService;

        public SearchController(ISearchService searchService)
        {
            this.searchService = searchService;
        }
        [HttpGet]
        public async Task<IActionResult> SearchItems([FromQuery]SearchParams searchParams)
        {
            

            var result = await  searchService.SearchItemAsync(searchParams);

            return Ok(new
            {
                result = result.Results,
                pageCount = result.PageCount,
                totalCount = result.TotalCount
            });
        }
    }
}
