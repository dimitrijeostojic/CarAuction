using AuctionService.Data;
using AuctionService.Entities.Domain;
using AuctionService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repositories.Implamentations
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly AuctionDbContext dbContext;

        public AuctionRepository(AuctionDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> CreateAuctionAsync(Auction auction)
        {
            await dbContext.AddAsync(auction);
            return await dbContext.SaveChangesAsync() > 0;

        }

        public async Task<Auction?> DeleteAuctionAsync(Guid id)
        {
            var auction = await dbContext.Auctions.FirstOrDefaultAsync(x=>x.Id == id);
            if (auction == null)
            {
                return null;
            }
            dbContext.Auctions.Remove(auction);
            var result = await dbContext.SaveChangesAsync() > 0;
            if (!result)
            {
                throw new Exception("Problem deleting auction");
            }
                return auction;
        }

        public async Task<List<Auction>> GetAllAuctionsAsync()
        {
            return await dbContext.Auctions.Include(x => x.Item).OrderBy(x => x.Item.Make).ToListAsync();
        }

        public async Task<Auction?> GetAuctionByIdAsync(Guid id)
        {
            var auction = await dbContext.Auctions.FirstOrDefaultAsync(x => x.Id == id);
            if (auction == null)
            {
                return null;
            }
            return auction;
        }

        public async Task<Auction?> UpdateAuctionAsync(Guid id, Auction auction)
        {
            var existingAuction = await dbContext.Auctions.Include(x=>x.Item).FirstOrDefaultAsync(x => x.Id == id);
            if (existingAuction == null)
            {
                return null;
            }
            existingAuction.Item.Make = auction.Item.Make ?? existingAuction.Item.Make;
            existingAuction.Item.Model = auction.Item.Model ?? existingAuction.Item.Model;
            existingAuction.Item.Color = auction.Item.Color ?? existingAuction.Item.Color;
            existingAuction.Item.Mileage = auction.Item.Mileage;
            existingAuction.Item.Year = auction.Item.Year;
            var result = await dbContext.SaveChangesAsync() > 0;
            if (!result)
            {
                throw new Exception("Auction hasn't been updated!");
            }
            return existingAuction;

        }
    }
}
