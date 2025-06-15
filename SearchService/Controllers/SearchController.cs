using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;
using SearchService.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace SearchService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService searchService;
        private readonly ILogger<SearchController> logger;

        public SearchController(ISearchService searchService, ILogger<SearchController> logger)
        {
            this.searchService = searchService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> SearchItems([FromQuery] SearchParams searchParams)
        {
            try
            {
                logger.LogInformation($"Searching items with parameters: {searchParams}");

                var result = await searchService.SearchItemAsync(searchParams);

                logger.LogInformation($"Search completed. Found {result.TotalCount} results.");

                return Ok(new
                {
                    result = result.Results,
                    pageCount = result.PageCount,
                    totalCount = result.TotalCount
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while searching items with parameters: {searchParams}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
