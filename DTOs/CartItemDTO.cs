namespace EAD_Backend_Application__.NET.DTOs
{
    public class CartItemDTO
    {
        public string? CartId { get; set; }
        public string? ImageUri { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public Double Price { get; set; } = 0.0;
        public Double Discount { get; set; } = 0.0;
        public int Quantity { get; set; } = 0;
        public string ProductId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

        public CartItemDTO()
        {

        }
    }
}
