using AuctionService.Data;
using AuctionService.Entities.Domain;
using AuctionService.Entities.DTOs;
using AuctionService.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Services.Implementations
{
    public class AuctionsService : IAuctionsService
    {
        private readonly IMapper mapper;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly AuctionDbContext dbContext;

        public AuctionsService(IMapper mapper, IPublishEndpoint publishEndpoint, AuctionDbContext dbContext)
        {
            this.mapper = mapper;
            this.publishEndpoint = publishEndpoint;
            this.dbContext = dbContext;
        }

        public async Task<AuctionDto?> CreateAuctionAsync(CreateAuctionDto createAuctionDto)
        {
            var auction = mapper.Map<Auction>(createAuctionDto);
            //TODO: add current user as seller
            auction.Seller = "test";

            var newAuction = mapper.Map<AuctionDto>(auction);
            await publishEndpoint.Publish(mapper.Map<AuctionCreated>(newAuction));

            await dbContext.AddAsync(auction);
            var result = await dbContext.SaveChangesAsync() > 0;

            return result ? mapper.Map<AuctionDto>(auction) : null;
        }

        public async Task<AuctionDto?> DeleteAuctionAsync(Guid id)
        {
            //TODO: check seller == username

            var auction = await dbContext.Auctions.FirstOrDefaultAsync(x => x.Id == id);
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
            return mapper.Map<AuctionDto>(auction);

        }

        public async Task<List<AuctionDto>> GetAllAuctionAsync(DateTime? date)
        {
            //domain
            var query = dbContext.Auctions.OrderBy(x => x.Item.Make).AsQueryable();
            //if (!string.IsNullOrEmpty(date))
            //{
            //    query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
            //}
            if (date.HasValue)
            {
                query = query.Where(x => x.UpdatedAt > date.Value.ToUniversalTime());
            }
            //return await dbContext.Auctions.Include(x => x.Item).OrderBy(x => x.Item.Make).ToListAsync();

            return await query.ProjectTo<AuctionDto>(mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<AuctionDto?> GetAuctionByIdAsync(Guid id)
        {
            var auction = await dbContext.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);
            if (auction == null)
            {
                return null;
            }
            return mapper.Map<AuctionDto>(auction);
        }

        public async Task<AuctionDto?> UpdateAuctionAsync(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            var updateAuctionDomain = mapper.Map<Auction>(updateAuctionDto);
            var existingAuction = await dbContext.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);
            if (existingAuction == null)
            {
                return null;
            }
            //TODO: check seller == username

            existingAuction.Item.Make = updateAuctionDomain.Item.Make ?? existingAuction.Item.Make;
            existingAuction.Item.Model = updateAuctionDomain.Item.Model ?? existingAuction.Item.Model;
            existingAuction.Item.Color = updateAuctionDomain.Item.Color ?? existingAuction.Item.Color;
            existingAuction.Item.Mileage = updateAuctionDomain.Item.Mileage;
            existingAuction.Item.Year = updateAuctionDomain.Item.Year;

            await publishEndpoint.Publish<AuctionUpdated>(mapper.Map<AuctionUpdated>(existingAuction));
            var result = await dbContext.SaveChangesAsync() > 0;
            if (!result)
            {
                throw new Exception("Auction hasn't been updated!");
            }
            return mapper.Map<AuctionDto>(existingAuction);
        }
    }
}
