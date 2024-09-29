namespace EAD_Backend_Application__.NET.DTOs
{
    public class OrderDTO
    {
        public string OrderId { get; set; } = string.Empty;
        public string? ProductImageResId { get; set; }
        public string OrderDate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Double TotalOrderPrice { get; set; }

        public OrderDTO()
        {
            
        }
    }
}
