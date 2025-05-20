using System.ComponentModel.DataAnnotations;

namespace AuctionService.Entities.Domain
{
    public class Auction
    {
        [Key]
        public Guid Id { get; set; }
        public int ReservePrice { get; set; }
        public int MyProperty { get; set; } = 0;
        public string Seller { get; set; }
        public string? Winner { get; set; }
        public int? SoldAmount { get; set; }
        public int? CurrentHighBid { get; set; }
        public DateTime CreatedAt { get; set; } = new DateTime(2025, 4, 29);
        public DateTime UpdatedAt { get; set; } = new DateTime(2025, 4, 29);
        public DateTime AuctionEnd { get; set; }
        public Status Status { get; set; }
        public Guid ItemId { get; set; }

        //nav property
        public Item Item { get; set; }
    }
}
