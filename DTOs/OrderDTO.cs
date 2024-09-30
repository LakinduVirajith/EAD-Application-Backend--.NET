namespace EAD_Backend_Application__.NET.DTOs
{
    public class OrderDTO
    {
        public required string OrderId { get; set; } = string.Empty;
        public string? ImageUri { get; set; }
        public required string OrderDate { get; set; } = string.Empty;
        public required string Status { get; set; } = string.Empty;
        public required Double TotalOrderPrice { get; set; }
    }
}
