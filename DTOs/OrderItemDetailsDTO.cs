namespace EAD_Backend_Application__.NET.DTOs
{
    public class OrderItemDetailsDTO
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? ImageResId { get; set; }
        public Double Price { get; set; }
        public string Quantity { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;

        public OrderItemDetailsDTO()
        {
            
        }
    }
}
