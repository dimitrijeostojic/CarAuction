using AuctionService.Entities.Domain;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data
{
    public class AuctionDbContext : DbContext
    {
        public AuctionDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();

            modelBuilder.Entity<Auction>()
            .HasOne(a => a.Item)
            .WithOne(i => i.Auction)
            .HasForeignKey<Auction>(a => a.ItemId);



            var items = new List<Item>()
            {
                new Item { Id = Guid.Parse("247f3fd9-d34d-4f72-8f2c-c38a4be7e3a4"), Make = "Ford", Model = "GT", Color = "White", Mileage = 50000, Year = 2020, ImageUrl = "https://cdn.pixabay.com/photo/2016/05/06/16/32/car-1376190_960_720.jpg" },
                new Item { Id = Guid.Parse("1e64b61f-56a2-41c4-84b2-7db83be6eb5a"), Make = "Bugatti", Model = "Veyron", Color = "Black", Mileage = 15035, Year = 2018, ImageUrl = "https://cdn.pixabay.com/photo/2012/05/29/00/43/car-49278_960_720.jpg" },
                new Item { Id = Guid.Parse("d166d7c7-ec02-4b1d-9f0a-0841c03c2374"), Make = "Ford", Model = "Mustang", Color = "Black", Mileage = 65125, Year = 2023, ImageUrl = "https://cdn.pixabay.com/photo/2012/11/02/13/02/car-63930_960_720.jpg" },
                new Item { Id = Guid.Parse("eab59875-7c85-457d-b5b2-ad6e5024b0d7"), Make = "Mercedes", Model = "SLK", Color = "Silver", Mileage = 15001, Year = 2020, ImageUrl = "https://cdn.pixabay.com/photo/2016/04/17/22/10/mercedes-benz-1335674_960_720.png" },
                new Item { Id = Guid.Parse("12d87902-4afc-415d-bc14-c44724f16954"), Make = "BMW", Model = "X1", Color = "White", Mileage = 90000, Year = 2017, ImageUrl = "https://cdn.pixabay.com/photo/2017/08/31/05/47/bmw-2699538_960_720.jpg" },
                new Item { Id = Guid.Parse("a2510c14-f119-4717-b4d5-c3eefe4b5782"), Make = "Ferrari", Model = "Spider", Color = "Red", Mileage = 50000, Year = 2015, ImageUrl = "https://cdn.pixabay.com/photo/2017/11/09/01/49/ferrari-458-spider-2932191_960_720.jpg" },
                new Item { Id = Guid.Parse("d3f9cf03-8a07-493d-9945-2cba4e3e1360"), Make = "Ferrari", Model = "F-430", Color = "Red", Mileage = 5000, Year = 2022, ImageUrl = "https://cdn.pixabay.com/photo/2017/11/08/14/39/ferrari-f430-2930661_960_720.jpg" },
                new Item { Id = Guid.Parse("a899e12c-3ddd-4cab-ae66-d6031a4b626f"), Make = "Audi", Model = "R8", Color = "White", Mileage = 10050, Year = 2021, ImageUrl = "https://cdn.pixabay.com/photo/2019/12/26/20/50/audi-r8-4721217_960_720.jpg" },
                new Item { Id = Guid.Parse("d90c328e-a26c-492d-b7c5-3ec037d1919d"), Make = "Audi", Model = "TT", Color = "Black", Mileage = 25400, Year = 2020, ImageUrl = "https://cdn.pixabay.com/photo/2016/09/01/15/06/audi-1636320_960_720.jpg" },
                new Item { Id = Guid.Parse("9be53e24-81e5-4e38-ab15-c14b2920c8ea"), Make = "Ford", Model = "Model T", Color = "Rust", Mileage = 150150, Year = 1938, ImageUrl = "https://cdn.pixabay.com/photo/2017/08/02/19/47/vintage-2573090_960_720.jpg" }
            };

            var auctions = new List<Auction>()
            {
                new Auction { Id = Guid.Parse("afbee524-5972-4075-8800-7d1f9d7b0a0c"), Status = Status.Finished, ReservePrice = 20000, Seller = "bob", AuctionEnd = new DateTime(2025, 5, 10), ItemId = items[0].Id},
                new Auction { Id = Guid.Parse("c8c3ec17-01bf-49db-82aa-1ef80b833a9f"), Status = Status.Finished, ReservePrice = 90000, Seller = "alice", AuctionEnd = new DateTime(2025, 6, 30), ItemId = items[1].Id },
                new Auction { Id = Guid.Parse("bbab4d5a-8565-48b1-9450-5ac2a5c4a654"), Status = Status.Finished, Seller = "bob", AuctionEnd = new DateTime(2025, 5, 4), ItemId = items[2].Id},
                new Auction { Id = Guid.Parse("155225c1-4448-4066-9886-6786536e05ea"), Status = Status.ReserveNotMet, ReservePrice = 50000, Seller = "tom", AuctionEnd = new DateTime(2025, 4, 15), ItemId = items[3].Id },
                new Auction { Id = Guid.Parse("466e4744-4dc5-4987-aae0-b621acfc5e39"), Status = Status.Finished, ReservePrice = 20000, Seller = "alice", AuctionEnd = new DateTime(2025, 5, 30), ItemId = items[4].Id},
                new Auction { Id = Guid.Parse("dc1e4071-d19d-459b-b848-b5c3cd3d151f"), Status = Status.Live, ReservePrice = 20000, Seller = "bob", AuctionEnd = new DateTime(2025, 8, 15), ItemId = items[5].Id },
                new Auction { Id = Guid.Parse("47111973-d176-4feb-848d-0ea22641c31a"), Status = Status.Live, ReservePrice = 150000, Seller = "alice", AuctionEnd = new DateTime(2025, 9, 13), ItemId = items[6].Id},
                new Auction { Id = Guid.Parse("6a5011a1-fe1f-47df-9a32-b5346b289391"), Status = Status.Live, Seller = "bob", AuctionEnd = new DateTime(2025, 5, 19), ItemId = items[7].Id },
                new Auction { Id = Guid.Parse("40490065-dac7-46b6-acc4-df507e0d6570"), Status = Status.Live, ReservePrice = 20000, Seller = "tom", AuctionEnd = new DateTime(2025, 11, 20), ItemId = items[8].Id},
                new Auction { Id = Guid.Parse("3659ac24-29dd-407a-81f5-ecfe6f924b9b"), Status = Status.Live, ReservePrice = 20000, Seller = "bob", AuctionEnd = new DateTime(2025, 10, 1), ItemId = items[9].Id }
            };

            modelBuilder.Entity<Item>().HasData(items);
            modelBuilder.Entity<Auction>().HasData(auctions);
        }
    }
}
