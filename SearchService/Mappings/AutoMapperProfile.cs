using AutoMapper;
using Contracts;
using SearchService.Models;

namespace SearchService.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AuctionCreated,Item>().ReverseMap();
            CreateMap<AuctionUpdated,Item>().ReverseMap();
        }
    }
}
