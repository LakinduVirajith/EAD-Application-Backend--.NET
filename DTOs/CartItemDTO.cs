namespace EAD_Backend_Application__.NET.DTOs
{
    public class CartItemDTO
    {
        public required string CartId { get; set; }
        public string? ImageUri { get; set; }
        public required string ProductName { get; set; }
        public required Double Price { get; set; } = 0.0;
        public required Double Discount { get; set; } = 0.0;
        public required string Size { get; set; }
        public required string Color { get; set; }
        public required int Quantity { get; set; } = 0;
        public required string ProductId { get; set; }
    }
}
