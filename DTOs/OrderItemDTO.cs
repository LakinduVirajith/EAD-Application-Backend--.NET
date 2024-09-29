namespace EAD_Backend_Application__.NET.DTOs
{
    public class OrderItemDTO
    {
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; } = 0;
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;

        public OrderItemDTO()
        {
            
        }
    }
}
