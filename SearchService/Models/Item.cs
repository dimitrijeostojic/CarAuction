using MongoDB.Entities;

namespace SearchService.Models
{
    public class Item : Entity
    {
        //ID has been provided from mongodb.entities
        //public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = new DateTime(2025, 4, 29);
        public DateTime UpdatedAt { get; set; } = new DateTime(2025, 4, 29);
        public DateTime AuctionEnd { get; set; }
        public string Seller { get; set; }
        public string? Winner { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public int Mileage { get; set; }
        public string ImageUrl { get; set; }
        public string Status { get; set; }
        public int ReservePrice { get; set; }
        public int? SoldAmount { get; set; }
        public int? CurrentHighBid { get; set; }
    }
}
