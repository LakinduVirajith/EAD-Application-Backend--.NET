namespace EAD_Backend_Application__.NET.DTOs
{
    public class OrderCancellationDTO
    {
        public string OrderId { get; set; } = string.Empty;
        public string CancellationReason { get; set; } = string.Empty;
    }
}
