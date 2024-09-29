namespace EAD_Backend_Application__.NET.DTOs
{
    public class ProductDTO
    {
        public string ProductId { get; set; } = string.Empty;
        public string? ImageUri { get; set; }
        public string Name { get; set; } = string.Empty;
        public Double Price { get; set; } = 0.0;
        public Double Discount { get; set; } = 0.0;
    }
}
