using AuctionService.Entities.Domain;
using AuctionService.Entities.DTOs;

namespace AuctionService.Repositories.Interfaces
{
    public interface IAuctionRepository
    {
        Task<List<Auction>> GetAllAuctionsAsync();
        Task<Auction?> GetAuctionByIdAsync(Guid id);
        Task<bool> CreateAuctionAsync(Auction auction);
        Task<Auction?> UpdateAuctionAsync(Guid id, Auction auction);
        Task<Auction?> DeleteAuctionAsync(Guid id);
    }
}
