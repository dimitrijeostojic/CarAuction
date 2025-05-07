using AuctionService.Data;
using AuctionService.Entities.Domain;
using AuctionService.Repositories.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repositories.Implamentations
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly AuctionDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IPublishEndpoint publishEndpoint;

        public AuctionRepository(AuctionDbContext dbContext, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.publishEndpoint = publishEndpoint;
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

            await publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

            var result = await dbContext.SaveChangesAsync() > 0;
            if (!result)
            {
                throw new Exception("Problem deleting auction");
            }
                return auction;
        }

        public async Task<IQueryable<Auction>> GetAllAuctionsAsync(DateTime? date)
        {

            var query = dbContext.Auctions.OrderBy(x => x.Item.Make).AsQueryable();
            //if (!string.IsNullOrEmpty(date))
            //{
            //    query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
            //}
            if (date.HasValue)
            {
                query = query.Where(x => x.UpdatedAt > date.Value.ToUniversalTime());
            }
                return query;
            //return await dbContext.Auctions.Include(x => x.Item).OrderBy(x => x.Item.Make).ToListAsync();
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

            await publishEndpoint.Publish<AuctionUpdated>(mapper.Map<AuctionUpdated>(existingAuction));
            var result = await dbContext.SaveChangesAsync() > 0;
            if (!result)
            {
                throw new Exception("Auction hasn't been updated!");
            }
            return existingAuction;

        }
    }
}
