using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.DTOs
{
    public class ProductStockDTO
    {
        [Required(ErrorMessage = "Product Id is required.")]
        public required string ProductId { get; set; }

        [Required(ErrorMessage = "Stock Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity must be a non-negative integer.")]
        public required int StockQuantity { get; set; } = 0;
    }
}
