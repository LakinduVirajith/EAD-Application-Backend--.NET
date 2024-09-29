using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.DTOs
{
    public class OrderItemDTO
    {
        [Required(ErrorMessage = "Product Id is required.")]
        public required string ProductId { get; set; }

        [Required(ErrorMessage = "Product Id is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative integer.")]
        public required int Quantity { get; set; } = 0;

        [Required(ErrorMessage = "Product Size is required.")]
        public required string Size { get; set; }

        [Required(ErrorMessage = "Product Color is required.")]
        public required string Color { get; set; }
    }
}
