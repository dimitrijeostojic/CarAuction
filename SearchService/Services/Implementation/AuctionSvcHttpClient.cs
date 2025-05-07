using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services.Implementation
{
    public class AuctionSvcHttpClient
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;

        public AuctionSvcHttpClient(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
        }

        public async Task<List<Item>> GetItemsForSearchDb()
        {
            var lastUpdated = await DB.Find<Item, string>()
                .Sort(x => x.Descending(x => x.UpdatedAt))
                .Project(x=>x.UpdatedAt.ToString())
                .ExecuteFirstAsync();

            Console.WriteLine(lastUpdated);

            return await httpClient.GetFromJsonAsync<List<Item>>(configuration["AuctionServiceUrl"]+"/api/auctions?date="+lastUpdated??"");
        }
    }
}
