using AuctionService.Entities.Domain;
using AuctionService.Entities.DTOs;
using AuctionService.Repositories.Interfaces;
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
        private readonly IAuctionRepository auctionRepository;
        private readonly IMapper mapper;
        private readonly IPublishEndpoint publishEndpoint;

        public AuctionsService(IAuctionRepository auctionRepository, IMapper mapper,IPublishEndpoint publishEndpoint)
        {
            this.auctionRepository = auctionRepository;
            this.mapper = mapper;
            this.publishEndpoint = publishEndpoint;
        }

        public async Task<AuctionDto?> CreateAuctionAsync(CreateAuctionDto createAuctionDto)
        {
            var auction = mapper.Map<Auction>(createAuctionDto);
            auction.Seller = "test";

            var newAuction = mapper.Map<AuctionDto>(auction);
            await publishEndpoint.Publish(mapper.Map<AuctionCreated>(newAuction));

            var result = await auctionRepository.CreateAuctionAsync(auction);

            return result ? mapper.Map<AuctionDto>(auction) : null;
        }

        public async Task<AuctionDto?> DeleteAuctionAsync(Guid id)
        {
            //TODO: check seller == username
            var auction = await auctionRepository.DeleteAuctionAsync(id);
            if (auction != null)
            {
                return mapper.Map<AuctionDto>(auction);
            }
            return null;
        }

        public async Task<List<AuctionDto>> GetAllAuctionAsync(DateTime? date)
        {
            //domain
            var query = await auctionRepository.GetAllAuctionsAsync(date);

            return await query.ProjectTo<AuctionDto>(mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<AuctionDto?> GetAuctionByIdAsync(Guid id)
        {
            var auction = await auctionRepository.GetAuctionByIdAsync(id);
            if (auction == null)
            {
                return null;
            }
            return mapper.Map<AuctionDto>(auction);
        }

        public async Task<AuctionDto?> UpdateAuctionAsync(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            var updateAuctionDomain = mapper.Map<Auction>(updateAuctionDto);
            ////TODO: check seller == username
            //if (updateAuctionDomain.Seller != "username")
            //{
            //    throw new Exception("Seller must be 'username'");
            //}

            var auctionDomain = await auctionRepository.UpdateAuctionAsync(id, updateAuctionDomain);
            if (auctionDomain == null)
            {
                return null;
            }
            return mapper.Map<AuctionDto>(auctionDomain);
        }
    }
}
