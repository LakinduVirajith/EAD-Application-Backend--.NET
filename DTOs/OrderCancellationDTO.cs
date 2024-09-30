using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.DTOs
{
    public class OrderCancellationDTO
    {
        [Required(ErrorMessage = "Order Id is required.")]
        public required string OrderId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cancellation Reason is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Cancellation Reason must be between 2 and 200 characters.")]
        public required string CancellationReason { get; set; } = string.Empty;
    }
}
