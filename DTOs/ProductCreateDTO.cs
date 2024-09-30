using EAD_Backend_Application__.NET.Helpers;
using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.DTOs
{
    public class ProductCreateDTO
    {
        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "Product Name must be between 2 and 40 characters.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Brand Name is required.")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "Brand Name must be between 2 and 40 characters.")]
        public required string Brand { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public required Double Price { get; set; } = 0.0;

        [Required(ErrorMessage = "Discount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Discount must be a positive value.")]
        public required Double Discount { get; set; } = 0.0;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, MinimumLength = 20, ErrorMessage = "Brand Name must be between 20 and 1000 characters.")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "Category must be between 2 and 40 characters.")]
        public required string Category { get; set; }

        [Required(ErrorMessage = "Stock Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity must be a non-negative integer.")]
        public required int StockQuantity { get; set; } = 0;

        [MinLengthCollection(1, ErrorMessage = "At least one size must be provided.")]
        public required List<string> Size { get; set; }

        [MinLengthCollection(1, ErrorMessage = "At least one color must be provided.")]
        public required List<string> Color { get; set; }
    }
}
