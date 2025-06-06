using AuctionService.Entities.Domain;
using AuctionService.Entities.DTOs;
using AutoMapper;
using Contracts;

namespace AuctionService.Mappings
{
    public class AutoMapperProfiles : Profile
    {

        public AutoMapperProfiles()
        {
            CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item).ReverseMap();
            CreateMap<Item, AuctionDto>().ReverseMap();

            CreateMap<CreateAuctionDto, Auction>().ForMember(d=>d.Item, o=>o.MapFrom(s=>s)).ReverseMap();
            CreateMap<CreateAuctionDto, Item>().ReverseMap();

            CreateMap<UpdateAuctionDto, Auction>().ForMember(d=>d.Item, o=>o.MapFrom(s=>s)).ReverseMap();
            CreateMap<UpdateAuctionDto, Item>().ReverseMap();

            CreateMap<AuctionDto, AuctionCreated>().ReverseMap();
            CreateMap<AuctionDto, AuctionUpdated>().ReverseMap();
            CreateMap<AuctionDto, AuctionDeleted>().ReverseMap();
        }
    }
}
