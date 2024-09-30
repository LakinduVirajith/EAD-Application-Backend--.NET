namespace EAD_Backend_Application__.NET.DTOs
{
    public class OrderItemDetailsDTO
    {
        public required string OrderItemId { get; set; }
        public required string ProductId { get; set; }
        public required string ProductName { get; set; }
        public string? ImageUri { get; set; }
        public required Double Price { get; set; }
        public required int Quantity { get; set; }
        public required string Size { get; set; }
        public required string Color { get; set; }
        public required string Status { get; set; }
    }
}
