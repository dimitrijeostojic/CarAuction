using AuctionService.Entities.DTOs;

namespace AuctionService.Services.Interfaces
{
    public interface IAuctionsService
    {
        Task<List<AuctionDto>> GetAllAuctionAsync();
        Task<AuctionDto?> GetAuctionByIdAsync(Guid id);
        Task<AuctionDto?> CreateAuctionAsync(CreateAuctionDto createAuctionDto);
        Task<AuctionDto?> UpdateAuctionAsync(Guid id, UpdateAuctionDto updateAuctionDto);
        Task<AuctionDto?> DeleteAuctionAsync(Guid id);
    }
}
