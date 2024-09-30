namespace EAD_Backend_Application__.NET.DTOs
{
    public class ProductDTO
    {
        public required string ProductId { get; set; }
        public string? ImageUri { get; set; }
        public required string Name { get; set; }
        public required Double Price { get; set; } = 0.0;
        public required Double Discount { get; set; } = 0.0;
        public required int StockQuantity { get; set; } = 0;
    }
}
