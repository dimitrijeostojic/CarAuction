using AuctionService.Entities.Domain;
using AuctionService.Entities.DTOs;

namespace AuctionService.Repositories.Interfaces
{
    public interface IAuctionRepository
    {
        Task<IQueryable<Auction>> GetAllAuctionsAsync(DateTime? date);
        Task<Auction?> GetAuctionByIdAsync(Guid id);
        Task<bool> CreateAuctionAsync(Auction auction);
        Task<Auction?> UpdateAuctionAsync(Guid id, Auction auction);
        Task<Auction?> DeleteAuctionAsync(Guid id);
    }
}
