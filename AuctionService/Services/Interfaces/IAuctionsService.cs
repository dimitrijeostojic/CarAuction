using AuctionService.Entities.DTOs;
using System.Security.Claims;

namespace AuctionService.Services.Interfaces
{
    public interface IAuctionsService
    {
        Task<List<AuctionDto>> GetAllAuctionAsync(DateTime? date);
        Task<AuctionDto?> GetAuctionByIdAsync(Guid id);
        Task<AuctionDto?> CreateAuctionAsync(CreateAuctionDto createAuctionDto, ClaimsPrincipal user);
        Task<AuctionDto?> UpdateAuctionAsync(Guid id, UpdateAuctionDto updateAuctionDto, ClaimsPrincipal user);
        Task<AuctionDto?> DeleteAuctionAsync(Guid id, ClaimsPrincipal user);
    }
}
