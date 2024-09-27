namespace EAD_Backend_Application__.NET.DTOs
{
    public class ProductDetailsDTO
    {
        public string? ImageUri { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public Double Price { get; set; } = 0.0;
        public Double Discount { get; set; } = 0.0;
        public List<string> Size { get; set; } = new List<string>();
        public List<string> Color { get; set; } = new List<string>();
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int StockQuantity { get; set; } = 0;
        public bool IsVisible { get; set; } = true;

        public ProductDetailsDTO()
        {
            
        }
    }
}
