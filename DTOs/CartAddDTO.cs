using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.DTOs
{
    public class CartAddDTO
    {
        [Required(ErrorMessage = "Product Id is required.")]
        public required string productId { get; set; }

        [Required(ErrorMessage = "Size is required.")]
        public required string Size { get; set; }

        [Required(ErrorMessage = "Color is required.")]
        public required string Color { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative integer.")]
        public required int Quantity { get; set; } = 0;
    }
}
